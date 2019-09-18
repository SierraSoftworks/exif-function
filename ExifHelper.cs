using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ImageMagick;
using Newtonsoft.Json;

namespace exif_function
{
    public static class ExifHelper
    {
        public static IDictionary<string, string> GetMetadata(Stream imageStream)
        {

            try
            {
                using (var image = new MagickImage(imageStream))
                {
                    var exif = image.GetExifProfile();

                    if (exif == null)
                        return null;

                    var metadata = new Dictionary<string, string>();
                    foreach (var value in exif.Values)
                    {
                        switch (value.Tag)
                        {
                            case ExifTag.DateTime:
                                {
                                    var dateTime = DateTime.ParseExact(value.Value.ToString(), "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                                    metadata["DateTime"] = dateTime.ToString("yyyy-MM-ddTHH-mm-ss");
                                    metadata["Date"] = dateTime.ToString("yyyy-MM-dd");
                                    metadata["Time"] = dateTime.ToString("HH-mm-ss");
                                    continue;
                                }
                        }

                        switch (value.DataType)
                        {
                            case ExifDataType.String:
                                metadata[value.Tag.ToString()] = (string)value.Value;
                                break;
                            case ExifDataType.Rational:
                                {
                                    if (value.Value is Rational rat)
                                        metadata[value.Tag.ToString()] = rat.ToString();
                                    else if (value.Value is Rational[] rata)
                                        metadata[value.Tag.ToString()] = string.Join(", ", rata.Select(r => r.ToString()));
                                    break;
                                }
                            case ExifDataType.SignedRational:
                                {
                                    if (value.Value is SignedRational rat)
                                        metadata[value.Tag.ToString()] = rat.ToString();
                                    else if (value.Value is SignedRational[] rata)
                                        metadata[value.Tag.ToString()] = string.Join(", ", rata.Select(r => r.ToString()));
                                    break;
                                }
                            default:
                                metadata[value.Tag.ToString()] = JsonConvert.SerializeObject(value.Value).Trim('"');
                                break;

                        }

                    }

                    return metadata;
                }
            }
            catch (MagickWarningException)
            {
                return null;
            }
            catch (MagickErrorException)
            {
                return null;
            }
        }
    }
}