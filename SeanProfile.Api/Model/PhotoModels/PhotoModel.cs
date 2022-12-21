namespace SeanProfile.Api.Model
{
    public class PhotoModel
    {
        public int PhotoId { get; set; }
        public string PhotoUrl { get; set; } = "";
        public DateTime InsertedDateTime { get; set; }
        public bool PermissionToView { get; set; }
        public int InsertedByUserId { get; set; }
        public string PhotoName { get; set; } = "";
        public string PhotoThumb { get; set; } = "";
        public string PhotoSRC { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }


    }
}
