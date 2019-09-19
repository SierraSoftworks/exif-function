using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using Newtonsoft.Json;

namespace exif_function
{
    public static class ExifHelper
    {
        public static IDictionary<string, string> GetMetadata(Stream imageStream)
        {

            try
            {
                using (var image = Image.Load(imageStream))
                {
                    var exif = image.Metadata.ExifProfile;

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
                            case ExifDataType.Ascii:
                                metadata[value.Tag.ToString()] = (string)value.Value;
                                break;
                            case ExifDataType.Rational:
                                {
                                    if (value.Value is double rat)
                                        metadata[value.Tag.ToString()] = rat.ToString();
                                    else if (value.Value is double[] rata)
                                        metadata[value.Tag.ToString()] = string.Join(", ", rata.Select(r => r.ToString()));
                                    break;
                                }
                            case ExifDataType.SignedRational:
                                {
                                    if (value.Value is double rat)
                                        metadata[value.Tag.ToString()] = rat.ToString();
                                    else if (value.Value is double[] rata)
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
            catch (ImageFormatException)
            {
                return null;
            }
            catch (ImageProcessingException)
            {
                return null;
            }
        }
    }
}