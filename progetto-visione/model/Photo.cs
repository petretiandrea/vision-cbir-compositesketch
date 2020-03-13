using CsvHelper;
using CsvHelper.Configuration;
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
        public int Id { get; set; }
        public string Path { get;  set; }
        public Gender Gender { get;  set; }

        public PhotoMetadata(int id, string path, Gender gender)
        {
            Id = id;
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Gender = gender;
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
            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                var photos = csv.GetRecords<PhotoMetadata>().ToList();
                if(useCsvPath)
                {
                    var csvRoot = Path.GetDirectoryName(csvFile);
                    return photos.Select(p => new PhotoMetadata(p.Id, Path.Combine(csvRoot, p.Path), p.Gender)).ToList();
                }
                return photos;
            }
        }
    }
}
