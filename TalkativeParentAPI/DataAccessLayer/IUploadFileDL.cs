using Org.BouncyCastle.Asn1.Ocsp;
using System.IO;
using System.Threading.Tasks;
using UploaderSheet_StudentMark.CommonLayer.Model;

namespace UploaderSheet_StudentMark.DataAccessLayer
{
    public interface IUploadFileDL
    {
        public Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, Stream fileStream);
        public Task<UploadCSVFileResponse> UploadCSVFile(UploadCSVFileRequest request, Stream fileStream);

        //Student Register Bulk Uploder Jaliya 07/09/2023
        public Task<UploadXMLFileResponse> StudentRegisterUploadXMLFile(StudentRegistrationUploadXMLFileRequest request, Stream fileStream);
        public Task<UploadCSVFileResponse> StudentRegisterUploadCSVFile(StudentRegistrationUploadCSVFileRequest request, Stream fileStream);
        public Task<UploadXMLFileResponse> SubjectUploadFile(NewUploadXMLFileRequest request, Stream fileStream);

        public Task<UploadXMLFileResponse> SubjectwithSubsubjectUploadFile(NewUploadXMLFileRequest request, Stream fileStream);
        public Task<UploadXMLFileResponse> StudentTeacherCommentUploadFile(StudentTeacherCommentUploadXMLFileRequest request, Stream fileStream);
    }
}
