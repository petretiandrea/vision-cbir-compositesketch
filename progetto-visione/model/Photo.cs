
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.Model
{
    public enum Gender
    {
        UNKNOWN, MALE, FEMALE
    };

    public class PhotoMetadata
    {
        public string Id { get; private set; }
        public Gender Gender { get; private set; }
        public string DatasetPath { get; private set; }
        public string AbsolutePath { get; private set; }

        public PhotoMetadata(string id, Gender gender, string datasetPath, string absolutePath)
        {
            Id = id;
            Gender = gender;
            DatasetPath = datasetPath;
            AbsolutePath = absolutePath ?? throw new ArgumentNullException(nameof(absolutePath));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PhotoMetadata))
            {
                return false;
            }

            var metadata = (PhotoMetadata)obj;
            return Id == metadata.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
    }

    public static class PhotoMetadataCsv
    {

        public static List<PhotoMetadata> FromCSV(string csvFile, bool useCsvPath = true)
        {
            using (var reader = new CsvReader(new StreamReader(csvFile), true))
            {
                var photos = new List<PhotoMetadata>();
                reader.GetFieldHeaders();
                while (reader.ReadNextRecord())
                {
                    photos.Add(Deserializer(csvFile, reader));
                }
                return photos;
            }
        }

        public static Func<string, CsvReader, PhotoMetadata> Deserializer = (filename, columns) =>
        {
            var absolute = Path.Combine(Path.GetDirectoryName(filename), columns[2]).Replace("/", @"\");
            return new PhotoMetadata(columns[0], Utils.ParseEnum<Gender>(columns[1]), columns[2], absolute);
        };

        public static Func<PhotoMetadata, string, string[]> Serializer = (photo, filename) =>
        {
            return new string[]
            {
                photo.Id,
                Enum.GetName(typeof(Gender), photo.Gender),
                photo.DatasetPath
            };
        };
    }
}
