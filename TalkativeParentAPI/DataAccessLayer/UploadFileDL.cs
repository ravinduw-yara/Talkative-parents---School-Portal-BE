using ExcelDataReader;
using LumenWorks.Framework.IO.Csv;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Formats.Asn1;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UploaderSheet_StudentMark.CommonLayer.Model;
using UploaderSheet_StudentMark.CommonUtility;
namespace UploaderSheet_StudentMark.DataAccessLayer
{
    public class UploadFileDL : IUploadFileDL
    {
        public readonly IConfiguration _configuration;
        public readonly SqlConnection _sqlConnection;
        public UploadFileDL(IConfiguration configuration)
        {
            _configuration = configuration;
            //_mySqlConnection = new MySqlConnection(_configuration["ConnectionStrings:SqlServerDBConnectionString"]);
            _sqlConnection = new SqlConnection(_configuration["ConnectionStrings:TpConnectionString"]);
        }


        //subject comment uploader
        public async Task<UploadXMLFileResponse> StudentTeacherCommentUploadFile(StudentTeacherCommentUploadXMLFileRequest request, Stream fileStream)
        {
            UploadXMLFileResponse response = new() { IsSuccess = true, Message = "Successful" };
            List<StudentTeacherCommentParameter> Parameters = new();

            try
            {
                if (_sqlConnection.State != ConnectionState.Open)
                    await _sqlConnection.OpenAsync();

                string fileExtension = Path.GetExtension(request.File.FileName).ToLower();
                DataTable dataTable = new();

                if (fileExtension == ".xlsx")
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(fileStream);
                    DataSet dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });
                    dataTable = dataSet.Tables[0];
                }
                else if (fileExtension == ".csv")
                {
                    using var reader = new StreamReader(fileStream);
                    using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                    using var dataReader = new CsvHelper.CsvDataReader(csv);
                    dataTable.Load(dataReader);
                }
                else
                {
                    response.Message = "Unsupported file format. Please upload .xlsx or .csv";
                    return response;
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    StudentTeacherCommentParameter rows = new()
                    {
                        RegistrationId = row[0] != DBNull.Value ? row[0].ToString() : null,
                        StandardName = row[2] != DBNull.Value ? row[2].ToString() : null,
                        SectionName = row[3] != DBNull.Value ? row[3].ToString() : null,
                        OverallComments = row[4] != DBNull.Value ? row[4].ToString() : null,
                        OverallCommentstwo = row[5] != DBNull.Value ? row[5].ToString() : null,
                        OverallCommenthree = row[6] != DBNull.Value ? row[6].ToString() : null
                    };

                    Parameters.Add(rows);
                }

                if (Parameters.Count > 0)
                {
                    string SqlQuery = "sp_StudentTeacherCommentBulkUpload";

                    foreach (StudentTeacherCommentParameter rows in Parameters)
                    {
                        using SqlCommand sqlCommand = new(SqlQuery, _sqlConnection)
                        {
                            CommandType = CommandType.StoredProcedure,
                            CommandTimeout = 180
                        };

                        sqlCommand.Parameters.AddWithValue("@Code", rows.RegistrationId);
                        sqlCommand.Parameters.AddWithValue("@StandardName", rows.StandardName);
                        sqlCommand.Parameters.AddWithValue("@SectionName", rows.SectionName);
                        sqlCommand.Parameters.AddWithValue("@OverallComments", rows.OverallComments ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@OverallCommentstwo", string.IsNullOrEmpty(rows.OverallCommentstwo) ? (object)DBNull.Value : rows.OverallCommentstwo);
                        sqlCommand.Parameters.AddWithValue("@OverallCommenthree", string.IsNullOrEmpty(rows.OverallCommenthree) ? (object)DBNull.Value : rows.OverallCommenthree);
                        sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);
                        sqlCommand.Parameters.AddWithValue("@LevelId", request.LevelId);
                        sqlCommand.Parameters.AddWithValue("@SemesterId", request.SemesterId);
                        sqlCommand.Parameters.AddWithValue("@ExameId", request.ExameId);

                        int Status = await sqlCommand.ExecuteNonQueryAsync();
                        if (Status <= 0)
                        {
                            response.Message += $"\nQuery not executed for Student: {rows.RegistrationId}.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message;
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                    await _sqlConnection.DisposeAsync();
                }
            }

            return response;
        }


        //Student Suject new Mark Bulk Uploder Jaliya
        public async Task<UploadXMLFileResponse> SubjectwithSubsubjectUploadFile(NewUploadXMLFileRequest request, Stream fileStream)
        {
            UploadXMLFileResponse response = new() { IsSuccess = true, Message = "Successful" };
            List<NewExcelBulkUploadParameter> Parameters = new();

            try
            {
                if (_sqlConnection.State != ConnectionState.Open)
                    await _sqlConnection.OpenAsync();

                string fileExtension = Path.GetExtension(request.File.FileName).ToLower();
                DataTable dataTable = new();

                if (fileExtension == ".xlsx")
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(fileStream);
                    DataSet dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });
                    dataTable = dataSet.Tables[0];
                }
                else if (fileExtension == ".csv")
                {
                    using var reader = new StreamReader(fileStream);
                    using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                    using var dataReader = new CsvHelper.CsvDataReader(csv);
                    dataTable.Load(dataReader);
                }
                else
                {
                    response.Message = "Unsupported file format. Please upload .xlsx or .csv";
                    return response;
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    NewExcelBulkUploadParameter rows = new()
                    {
                        RegistrationId = row[0] != DBNull.Value ? row[0].ToString() : null,
                        Grade = row[2] != DBNull.Value ? row[2].ToString() : null,
                        Section = row[3] != DBNull.Value ? row[3].ToString() : null,
                        SubjectId = row[4] != DBNull.Value ? Convert.ToInt32(row[4]) : (int?)null,
                        SubjectName = row[5] != DBNull.Value ? row[5].ToString() : null,
                        MainSubjectComment = row[11] != DBNull.Value ? row[11].ToString() : null
                    };

                    if (float.TryParse(row[6]?.ToString(), out float thoery)) rows.Thoery = thoery;
                    if (float.TryParse(row[7]?.ToString(), out float practical)) rows.Practical = practical;
                    if (float.TryParse(row[8]?.ToString(), out float mark3)) rows.SubSubjectMarks3 = mark3;
                    if (float.TryParse(row[9]?.ToString(), out float mark4)) rows.SubSubjectMarks4 = mark4;
                    if (float.TryParse(row[10]?.ToString(), out float mark5)) rows.SubSubjectMarks5 = mark5;

                    Parameters.Add(rows);
                }

                if (Parameters.Count > 0)
                {
                    string SqlQuery = "sp_NewStudentMarkBulkUploadNew";

                    foreach (NewExcelBulkUploadParameter rows in Parameters)
                    {
                        using SqlCommand sqlCommand = new(SqlQuery, _sqlConnection)
                        {
                            CommandType = CommandType.StoredProcedure,
                            CommandTimeout = 180
                        };

                        sqlCommand.Parameters.AddWithValue("@Code", rows.RegistrationId);
                        sqlCommand.Parameters.AddWithValue("@StandardName", rows.Grade);
                        sqlCommand.Parameters.AddWithValue("@SectionName", rows.Section);
                        sqlCommand.Parameters.AddWithValue("@SubjectId", rows.SubjectId);
                        sqlCommand.Parameters.AddWithValue("@SubjectName", rows.SubjectName);

                        sqlCommand.Parameters.AddWithValue("@Mark1", rows.Thoery ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Mark2", rows.Practical ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Mark3", rows.SubSubjectMarks3 ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Mark4", rows.SubSubjectMarks4 ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Mark5", rows.SubSubjectMarks5 ?? (object)DBNull.Value);

                        sqlCommand.Parameters.AddWithValue("@Comment", rows.MainSubjectComment ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);
                        sqlCommand.Parameters.AddWithValue("@LevelId", request.LevelId);
                        sqlCommand.Parameters.AddWithValue("@SemesterId", request.SemesterId);
                        sqlCommand.Parameters.AddWithValue("@ExameId", request.ExameId);

                        int Status = await sqlCommand.ExecuteNonQueryAsync();
                        if (Status <= 0)
                        {
                            response.Message += $"\nQuery not executed for Subject: {rows.SubjectName} ({rows.SubjectId}).";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message;
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                    await _sqlConnection.DisposeAsync();
                }
            }

            return response;
        }



        public async Task<UploadXMLFileResponse> SubjectUploadFile(NewUploadXMLFileRequest request, Stream fileStream)
        {
            UploadXMLFileResponse response = new() { IsSuccess = true, Message = "Successful" };
            List<NewExcelBulkUploadParameter> Parameters = new();
            bool anyRowInserted = false;

            try
            {
                if (_sqlConnection.State != ConnectionState.Open)
                    await _sqlConnection.OpenAsync();

                string fileExtension = Path.GetExtension(request.File.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".csv")
                    return new UploadXMLFileResponse { IsSuccess = false, Message = "Incorrect File Format" };

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                DataTable dataTable;

                if (fileExtension == ".xlsx")
                {
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(fileStream);
                    DataSet dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });
                    dataTable = dataSet.Tables[0];
                }
                else
                {
                    using var reader = new StreamReader(fileStream);
                    using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                    using var dataReader = new CsvHelper.CsvDataReader(csv);
                    dataTable = new DataTable();
                    dataTable.Load(dataReader);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    try
                    {
                        string registrationId = row["RegistrationId"].ToString().Trim();
                        string grade = row["Grade"].ToString().Trim();
                        string section = row["Section"].ToString().Trim();

                        for (int i = 4; i < row.ItemArray.Length; i += 4)
                        {
                            if (row[i] == DBNull.Value) continue;

                            int? subjectId = row[i] != DBNull.Value ? Convert.ToInt32(row[i]) : (int?)null;
                            string subjectName = row[i + 1]?.ToString().Trim();
                            string rawMark = row[i + 2]?.ToString().Trim();

                            if (!float.TryParse(rawMark, out float parsedMark))
                            {
                                response.Message += $"\nData parsing error: Cannot convert '{rawMark}' to float in column {i + 2}.";
                                continue;
                            }

                            float? mainMark = parsedMark;
                            string subjectComment = row[i + 3]?.ToString().Trim();

                            Parameters.Add(new NewExcelBulkUploadParameter()
                            {
                                RegistrationId = registrationId,
                                Grade = grade,
                                Section = section,
                                SubjectId = subjectId,
                                SubjectName = subjectName,
                                Thoery = mainMark,
                                MainSubjectComment = subjectComment
                            });
                        }
                    }
                    catch (Exception rowEx)
                    {
                        response.Message += "\nData parsing error: " + rowEx.Message;
                    }
                }

                if (Parameters.Count > 0)
                {
                    foreach (var param in Parameters)
                    {
                        using SqlCommand sqlCommand = new("sp_NewStudentMarkBulkUploadNew", _sqlConnection)
                        {
                            CommandType = CommandType.StoredProcedure,
                            CommandTimeout = 180
                        };
                        sqlCommand.Parameters.Clear();
                        sqlCommand.Parameters.AddWithValue("@Code", param.RegistrationId);
                        sqlCommand.Parameters.AddWithValue("@StandardName", param.Grade);
                        sqlCommand.Parameters.AddWithValue("@SectionName", param.Section);
                        sqlCommand.Parameters.AddWithValue("@SubjectId", param.SubjectId);
                        sqlCommand.Parameters.AddWithValue("@SubjectName", param.SubjectName);
                        sqlCommand.Parameters.AddWithValue("@Mark1", param.Thoery ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Mark2", DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Mark3", DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Mark4", DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Mark5", DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Comment", param.MainSubjectComment ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);
                        sqlCommand.Parameters.AddWithValue("@LevelId", request.LevelId);
                        sqlCommand.Parameters.AddWithValue("@SemesterId", request.SemesterId);
                        sqlCommand.Parameters.AddWithValue("@ExameId", request.ExameId);

                        try
                        {
                            int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();
                            if (rowsAffected > 0)
                                anyRowInserted = true;
                            else
                                response.Message += $"\nNo rows affected for Subject: {param.SubjectName} ({param.SubjectId}).";
                        }
                        catch (SqlException sqlEx)
                        {
                            response.Message = "SQL Execution Error: " + sqlEx.Message;
                            return response;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "Unexpected error: " + ex.Message;
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                    await _sqlConnection.DisposeAsync();
                }
            }
            return response;
        }


        //Student Mark Bulk Uploder Jaliya 
        public async Task<UploadCSVFileResponse> UploadCSVFile(UploadCSVFileRequest request, Stream fileStream)
        {
            UploadCSVFileResponse response = new UploadCSVFileResponse();
            List<ExcelBulkUploadParameter> Parameters = new List<ExcelBulkUploadParameter>();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {
                if (request.File.FileName.ToLower().Contains(".csv"))
                {
                    DataTable value = new DataTable();

                    using (var csvReader = new CsvReader(new StreamReader(fileStream), true))
                    {
                        value.Load(csvReader);
                    }

                    for (int i = 0; i < value.Rows.Count; i++)
                    {
                        ExcelBulkUploadParameter rows = new ExcelBulkUploadParameter();

                        rows.RegistrationId = value.Rows[i][0] != DBNull.Value ? Convert.ToString(value.Rows[i][0]) : null;

                        if (float.TryParse(Convert.ToString(value.Rows[i][2]), out float thoery))
                            rows.Thoery = thoery;
                        if (float.TryParse(Convert.ToString(value.Rows[i][3]), out float practical))
                            rows.Practical = practical;
                        if (float.TryParse(Convert.ToString(value.Rows[i][4]), out float mark3))
                            rows.SubSubjectMarks3 = mark3;
                        if (float.TryParse(Convert.ToString(value.Rows[i][5]), out float mark4))
                            rows.SubSubjectMarks4 = mark4;
                        if (float.TryParse(Convert.ToString(value.Rows[i][6]), out float mark5))
                            rows.SubSubjectMarks5 = mark5;
                        rows.MainSubjectComment = value.Rows[i][7] != DBNull.Value ? Convert.ToString(value.Rows[i][7]) : null;
                        Parameters.Add(rows);
                    }

                    if (Parameters.Count > 0)
                    {
                        if (_sqlConnection.State != System.Data.ConnectionState.Open)
                        {
                            await _sqlConnection.OpenAsync();
                        }

                        string SqlQuery = "sp_StudentMarkBulkUpload";

                        foreach (ExcelBulkUploadParameter rows in Parameters)
                        {
                            using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                            {
                                sqlCommand.CommandType = CommandType.StoredProcedure;
                                sqlCommand.CommandTimeout = 180;
                                sqlCommand.Parameters.AddWithValue("@Code", rows.RegistrationId);

                                // Only add these parameters if they are not null and not equal to 0.
                                if (rows.Thoery != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark1", rows.Thoery);
                                }
                                if (rows.Practical != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark2", rows.Practical);
                                }
                                if (rows.SubSubjectMarks3 != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark3", rows.SubSubjectMarks3);
                                }
                                if (rows.SubSubjectMarks4 != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark4", rows.SubSubjectMarks4);
                                }
                                if (rows.SubSubjectMarks5 != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark5", rows.SubSubjectMarks5);
                                }

                                sqlCommand.Parameters.AddWithValue("@Comment", rows.MainSubjectComment);

                                sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);
                                sqlCommand.Parameters.AddWithValue("@LevelId", request.LevelId);
                                sqlCommand.Parameters.AddWithValue("@StandardId", request.StandardId);
                                sqlCommand.Parameters.AddWithValue("@SectionId", request.SectionId);
                                sqlCommand.Parameters.AddWithValue("@SemesterId", request.SemesterId);
                                sqlCommand.Parameters.AddWithValue("@SubjectId", request.SubjectId);
                                sqlCommand.Parameters.AddWithValue("@ExameId", request.ExameId);
                                int Status = await sqlCommand.ExecuteNonQueryAsync();
                                if (Status <= 0)
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Query Not Executed";
                                    return response;
                                }
                            }
                        }
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid File";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                    await _sqlConnection.DisposeAsync();
                }
            }

            return response;
        }


        //Student Mark Bulk Uploder Jaliya 
        public async Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, Stream fileStream)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();
            List<ExcelBulkUploadParameter> Parameters = new List<ExcelBulkUploadParameter>();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {
                if (_sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                if (request.File.FileName.ToLower().Contains(value: ".xlsx"))
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(fileStream);
                    DataSet dataSet = reader.AsDataSet(
                        new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = false,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }

                        });

                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        ExcelBulkUploadParameter rows = new ExcelBulkUploadParameter();

                        rows.RegistrationId = dataSet.Tables[0].Rows[i].ItemArray[0] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : null;

                        if (float.TryParse(dataSet.Tables[0].Rows[i].ItemArray[2]?.ToString(), out float thoery))
                            rows.Thoery = thoery;

                        if (float.TryParse(dataSet.Tables[0].Rows[i].ItemArray[3]?.ToString(), out float practical))
                            rows.Practical = practical;

                        if (float.TryParse(dataSet.Tables[0].Rows[i].ItemArray[4]?.ToString(), out float mark3))
                            rows.SubSubjectMarks3 = mark3;

                        if (float.TryParse(dataSet.Tables[0].Rows[i].ItemArray[5]?.ToString(), out float mark4))
                            rows.SubSubjectMarks4 = mark4;

                        if (float.TryParse(dataSet.Tables[0].Rows[i].ItemArray[6]?.ToString(), out float mark5))
                            rows.SubSubjectMarks5 = mark5;
                        rows.MainSubjectComment = dataSet.Tables[0].Rows[i].ItemArray[7] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[7]) : null;

                        Parameters.Add(rows);
                    }

                    if (Parameters.Count > 0)
                    {
                        string SqlQuery = "sp_StudentMarkBulkUpload";

                        foreach (ExcelBulkUploadParameter rows in Parameters)
                        {
                            using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                            {
                                sqlCommand.CommandType = CommandType.StoredProcedure;
                                sqlCommand.CommandTimeout = 180;
                                sqlCommand.Parameters.AddWithValue("@Code", rows.RegistrationId);

                                // Only add these parameters if they are not null and not equal to 0.
                                if (rows.Thoery != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark1", rows.Thoery);
                                }
                                if (rows.Practical != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark2", rows.Practical);
                                }
                                if (rows.SubSubjectMarks3 != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark3", rows.SubSubjectMarks3);
                                }
                                if (rows.SubSubjectMarks4 != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark4", rows.SubSubjectMarks4);
                                }
                                if (rows.SubSubjectMarks5 != null)
                                {
                                    sqlCommand.Parameters.AddWithValue("@Mark5", rows.SubSubjectMarks5);
                                }

                                sqlCommand.Parameters.AddWithValue("@Comment", rows.MainSubjectComment);
                                sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);
                                sqlCommand.Parameters.AddWithValue("@LevelId", request.LevelId);
                                sqlCommand.Parameters.AddWithValue("@StandardId", request.StandardId);
                                sqlCommand.Parameters.AddWithValue("@SectionId", request.SectionId);
                                sqlCommand.Parameters.AddWithValue("@SemesterId", request.SemesterId);
                                sqlCommand.Parameters.AddWithValue("@SubjectId", request.SubjectId);
                                sqlCommand.Parameters.AddWithValue("@ExameId", request.ExameId);

                                int Status = await sqlCommand.ExecuteNonQueryAsync();
                                if (Status <= 0)
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Query Not Executed";
                                    return response;
                                }
                            }
                        }
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Incorrect File";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                    await _sqlConnection.DisposeAsync();
                }
            }
            return response;
        }

        //July 3 2025
        public async Task<UploadXMLFileResponse> StudentRegisterUploadXMLFile(StudentRegistrationUploadXMLFileRequest request, Stream fileStream)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();
            List<StudentRegistrationParameter> Parameters = new List<StudentRegistrationParameter>();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {
                if (_sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                if (request.File.FileName.ToLower().Contains(value: ".xlsx"))
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(fileStream);
                    DataSet dataSet = reader.AsDataSet(
                        new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = false,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        StudentRegistrationParameter rows = new StudentRegistrationParameter();

                        rows.Code = dataSet.Tables[0].Rows[i].ItemArray[0] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : null;
                        rows.StudentFirstName = dataSet.Tables[0].Rows[i].ItemArray[1] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : null;
                        rows.StudentLastName = dataSet.Tables[0].Rows[i].ItemArray[2] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[2]) : null;

                        rows.StandardName = dataSet.Tables[0].Rows[i].ItemArray[3] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[3]) : null;
                        rows.SectionName = dataSet.Tables[0].Rows[i].ItemArray[4] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[4]) : null;

                        rows.StudentGenderId = dataSet.Tables[0].Rows[i].ItemArray[5] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[5]) : (int?)null;

                        rows.StudentEmail = dataSet.Tables[0].Rows[i].ItemArray[6] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[6]) : null;
                        rows.StudentIsHieduuser = dataSet.Tables[0].Rows[i].ItemArray[7] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[7]) : (int?)null;
                        rows.StudentHieduActive = dataSet.Tables[0].Rows[i].ItemArray[8] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[8]) : (int?)null;
                        rows.MotherFirstName = dataSet.Tables[0].Rows[i].ItemArray[9] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[9]) : null;
                        rows.MotherLastName = dataSet.Tables[0].Rows[i].ItemArray[10] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[10]) : null;

                        rows.MotherPhoneNumber = dataSet.Tables[0].Rows[i].ItemArray[11] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[11]) : null;
                        rows.MotherGenderId = dataSet.Tables[0].Rows[i].ItemArray[12] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[12]) : (int?)null;
                        rows.MotherEmail = dataSet.Tables[0].Rows[i].ItemArray[13] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[13]) : null;
                        rows.MotherIssmsuser = dataSet.Tables[0].Rows[i].ItemArray[14] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[14]) : (int?)null;
                        rows.MotherIshigheduser = dataSet.Tables[0].Rows[i].ItemArray[15] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[15]) : (int?)null;
                        rows.FatherFirstName = dataSet.Tables[0].Rows[i].ItemArray[16] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[16]) : null;
                        rows.FatherLastName = dataSet.Tables[0].Rows[i].ItemArray[17] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[17]) : null;

                        rows.FatherPhoneNumber = dataSet.Tables[0].Rows[i].ItemArray[18] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[18]) : null;
                        rows.FatherGenderId = dataSet.Tables[0].Rows[i].ItemArray[19] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[19]) : (int?)null;
                        rows.FatherEmail = dataSet.Tables[0].Rows[i].ItemArray[20] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[20]) : null;
                        rows.FatherIssmsuser = dataSet.Tables[0].Rows[i].ItemArray[21] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[21]) : (int?)null;
                        rows.FatherIshigheduser = dataSet.Tables[0].Rows[i].ItemArray[22] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[22]) : (int?)null;
                        Parameters.Add(rows);
                    }



                    if (Parameters.Count > 0)
                    {
                        string SqlQuery = "sp_StudentRegisterBulkUploderSheet";

                        foreach (StudentRegistrationParameter rows in Parameters)
                        {
                            StudentUploadStatus uploadStatus = new StudentUploadStatus
                            {
                                Code = rows.Code,
                                StudentFirstName = rows.StudentFirstName,
                                StudentLastName = rows.StudentLastName,
                                IsSuccess = true,
                                Message = "Uploaded successfully"
                            };

                            try
                            {
                                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                                {
                                    sqlCommand.CommandType = CommandType.StoredProcedure;
                                    sqlCommand.CommandTimeout = 180;
                                    sqlCommand.Parameters.AddWithValue("@Code", rows.Code);
                                    sqlCommand.Parameters.AddWithValue("@StudentFirstName", rows.StudentFirstName);
                                    sqlCommand.Parameters.AddWithValue("@StudentLastName", rows.StudentLastName);

                                    sqlCommand.Parameters.AddWithValue("@StandardName", rows.StandardName);
                                    sqlCommand.Parameters.AddWithValue("@SectionName", rows.SectionName);

                                    sqlCommand.Parameters.AddWithValue("@StudentGenderId", rows.StudentGenderId);

                                    sqlCommand.Parameters.AddWithValue("@StudentEmail", rows.StudentEmail);
                                    sqlCommand.Parameters.AddWithValue("@StudentIsHieduuser", rows.StudentIsHieduuser);
                                    sqlCommand.Parameters.AddWithValue("@StudentHieduActive", rows.StudentHieduActive);
                                    sqlCommand.Parameters.AddWithValue("@MotherFirstName", rows.MotherFirstName);
                                    sqlCommand.Parameters.AddWithValue("@MotherLastName", rows.MotherLastName);

                                    sqlCommand.Parameters.AddWithValue("@MotherPhoneNumber", rows.MotherPhoneNumber);
                                    sqlCommand.Parameters.AddWithValue("@MotherGenderId", rows.MotherGenderId);
                                    sqlCommand.Parameters.AddWithValue("@MotherEmail", rows.MotherEmail);
                                    sqlCommand.Parameters.AddWithValue("@MotherIssmsuser", rows.MotherIssmsuser);
                                    sqlCommand.Parameters.AddWithValue("@MotherIshigheduser", rows.MotherIshigheduser);
                                    sqlCommand.Parameters.AddWithValue("@FatherFirstName", rows.FatherFirstName);
                                    sqlCommand.Parameters.AddWithValue("@FatherLastName", rows.FatherLastName);

                                    sqlCommand.Parameters.AddWithValue("@FatherPhoneNumber", rows.FatherPhoneNumber);
                                    sqlCommand.Parameters.AddWithValue("@FatherGenderId", rows.FatherGenderId);
                                    sqlCommand.Parameters.AddWithValue("@FatherEmail", rows.FatherEmail);
                                    sqlCommand.Parameters.AddWithValue("@FatherIssmsuser", rows.FatherIssmsuser);
                                    sqlCommand.Parameters.AddWithValue("@FatherIshigheduser", rows.FatherIshigheduser);
                                    sqlCommand.Parameters.AddWithValue("@SchoolId", request.SchoolId);
                                    sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);

                                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                                    if (Status <= 0)
                                    {
                                        uploadStatus.IsSuccess = false;
                                        uploadStatus.Message = "Query Not Executed";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                uploadStatus.IsSuccess = false;
                                uploadStatus.Message = ex.Message;
                            }

                            response.UploadStatuses.Add(uploadStatus);
                        }
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Incorrect File";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                    await _sqlConnection.DisposeAsync();
                }
            }
            return response;
        }




        public async Task<UploadCSVFileResponse> StudentRegisterUploadCSVFile(StudentRegistrationUploadCSVFileRequest request, Stream fileStream)
        {
            UploadCSVFileResponse response = new UploadCSVFileResponse();
            List<StudentRegistrationParameter> Parameters = new List<StudentRegistrationParameter>();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {
                // ensure the connection is open just like in the XLSX version
                if (_sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                if (request.File.FileName.ToLower().Contains(".csv"))
                {
                    DataTable value = new DataTable();

                    using (var csvReader = new CsvReader(new StreamReader(fileStream), true))
                    {
                        value.Load(csvReader);
                    }

                    for (int i = 0; i < value.Rows.Count; i++)
                    {
                        StudentRegistrationParameter rows = new StudentRegistrationParameter();

                        rows.Code = value.Rows[i][0] != DBNull.Value ? Convert.ToString(value.Rows[i][0]) : null;
                        rows.StudentFirstName = value.Rows[i][1] != DBNull.Value ? Convert.ToString(value.Rows[i][1]) : null;
                        rows.StudentLastName = value.Rows[i][2] != DBNull.Value ? Convert.ToString(value.Rows[i][2]) : null;
                        rows.StandardName = value.Rows[i][3] != DBNull.Value ? Convert.ToString(value.Rows[i][3]) : null;
                        rows.SectionName = value.Rows[i][4] != DBNull.Value ? Convert.ToString(value.Rows[i][4]) : null;
                        rows.StudentGenderId = value.Rows[i][5] != DBNull.Value ? Convert.ToInt32(value.Rows[i][5]) : (int?)null;
                        rows.StudentEmail = value.Rows[i][6] != DBNull.Value ? Convert.ToString(value.Rows[i][6]) : null;
                        rows.StudentIsHieduuser = value.Rows[i][7] != DBNull.Value ? Convert.ToInt32(value.Rows[i][7]) : (int?)null;
                        rows.StudentHieduActive = value.Rows[i][8] != DBNull.Value ? Convert.ToInt32(value.Rows[i][8]) : (int?)null;
                        rows.MotherFirstName = value.Rows[i][9] != DBNull.Value ? Convert.ToString(value.Rows[i][9]) : null;
                        rows.MotherLastName = value.Rows[i][10] != DBNull.Value ? Convert.ToString(value.Rows[i][10]) : null;
                        rows.MotherPhoneNumber = value.Rows[i][11] != DBNull.Value ? Convert.ToString(value.Rows[i][11]) : null;
                        rows.MotherGenderId = value.Rows[i][12] != DBNull.Value ? Convert.ToInt32(value.Rows[i][12]) : (int?)null;
                        rows.MotherEmail = value.Rows[i][13] != DBNull.Value ? Convert.ToString(value.Rows[i][13]) : null;
                        rows.MotherIssmsuser = value.Rows[i][14] != DBNull.Value ? Convert.ToInt32(value.Rows[i][14]) : (int?)null;
                        rows.MotherIshigheduser = value.Rows[i][15] != DBNull.Value ? Convert.ToInt32(value.Rows[i][15]) : (int?)null;
                        rows.FatherFirstName = value.Rows[i][16] != DBNull.Value ? Convert.ToString(value.Rows[i][16]) : null;
                        rows.FatherLastName = value.Rows[i][17] != DBNull.Value ? Convert.ToString(value.Rows[i][17]) : null;
                        rows.FatherPhoneNumber = value.Rows[i][18] != DBNull.Value ? Convert.ToString(value.Rows[i][18]) : null;
                        rows.FatherGenderId = value.Rows[i][19] != DBNull.Value ? Convert.ToInt32(value.Rows[i][19]) : (int?)null;
                        rows.FatherEmail = value.Rows[i][20] != DBNull.Value ? Convert.ToString(value.Rows[i][20]) : null;
                        rows.FatherIssmsuser = value.Rows[i][21] != DBNull.Value ? Convert.ToInt32(value.Rows[i][21]) : (int?)null;
                        rows.FatherIshigheduser = value.Rows[i][22] != DBNull.Value ? Convert.ToInt32(value.Rows[i][22]) : (int?)null;

                        Parameters.Add(rows);
                    }

                    if (Parameters.Count > 0)
                    {
                        string SqlQuery = "sp_StudentRegisterBulkUploderSheet";

                        foreach (StudentRegistrationParameter rows in Parameters)
                        {
                            StudentUploadStatus uploadStatus = new StudentUploadStatus
                            {
                                Code = rows.Code,
                                StudentFirstName = rows.StudentFirstName,
                                StudentLastName = rows.StudentLastName,
                                IsSuccess = true,
                                Message = "Uploaded successfully"
                            };

                            try
                            {
                                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                                {
                                    sqlCommand.CommandType = CommandType.StoredProcedure;
                                    sqlCommand.CommandTimeout = 180;

                                    sqlCommand.Parameters.AddWithValue("@Code", rows.Code);
                                    sqlCommand.Parameters.AddWithValue("@StudentFirstName", rows.StudentFirstName);
                                    sqlCommand.Parameters.AddWithValue("@StudentLastName", rows.StudentLastName);
                                    sqlCommand.Parameters.AddWithValue("@StandardName", rows.StandardName);
                                    sqlCommand.Parameters.AddWithValue("@SectionName", rows.SectionName);
                                    sqlCommand.Parameters.AddWithValue("@StudentGenderId", rows.StudentGenderId);
                                    sqlCommand.Parameters.AddWithValue("@StudentEmail", rows.StudentEmail);
                                    sqlCommand.Parameters.AddWithValue("@StudentIsHieduuser", rows.StudentIsHieduuser);
                                    sqlCommand.Parameters.AddWithValue("@StudentHieduActive", rows.StudentHieduActive);
                                    sqlCommand.Parameters.AddWithValue("@MotherFirstName", rows.MotherFirstName);
                                    sqlCommand.Parameters.AddWithValue("@MotherLastName", rows.MotherLastName);
                                    sqlCommand.Parameters.AddWithValue("@MotherPhoneNumber", rows.MotherPhoneNumber);
                                    sqlCommand.Parameters.AddWithValue("@MotherGenderId", rows.MotherGenderId);
                                    sqlCommand.Parameters.AddWithValue("@MotherEmail", rows.MotherEmail);
                                    sqlCommand.Parameters.AddWithValue("@MotherIssmsuser", rows.MotherIssmsuser);
                                    sqlCommand.Parameters.AddWithValue("@MotherIshigheduser", rows.MotherIshigheduser);
                                    sqlCommand.Parameters.AddWithValue("@FatherFirstName", rows.FatherFirstName);
                                    sqlCommand.Parameters.AddWithValue("@FatherLastName", rows.FatherLastName);
                                    sqlCommand.Parameters.AddWithValue("@FatherPhoneNumber", rows.FatherPhoneNumber);
                                    sqlCommand.Parameters.AddWithValue("@FatherGenderId", rows.FatherGenderId);
                                    sqlCommand.Parameters.AddWithValue("@FatherEmail", rows.FatherEmail);
                                    sqlCommand.Parameters.AddWithValue("@FatherIssmsuser", rows.FatherIssmsuser);
                                    sqlCommand.Parameters.AddWithValue("@FatherIshigheduser", rows.FatherIshigheduser);
                                    sqlCommand.Parameters.AddWithValue("@SchoolId", request.SchoolId);
                                    sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);

                                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                                    if (Status <= 0)
                                    {
                                        uploadStatus.IsSuccess = false;
                                        uploadStatus.Message = "Query Not Executed";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                uploadStatus.IsSuccess = false;
                                uploadStatus.Message = ex.Message;
                            }

                            response.UploadStatuses.Add(uploadStatus);
                        }
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Incorrect File";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                    await _sqlConnection.DisposeAsync();
                }
            }

            return response;
        }


        //// student bulk upload 21/3/2025


        //public async Task<UploadXMLFileResponse> StudentRegisterUploadXMLFile(StudentRegistrationUploadXMLFileRequest request, Stream fileStream)
        //{
        //    UploadXMLFileResponse response = new UploadXMLFileResponse();
        //    List<StudentRegistrationParameter> Parameters = new List<StudentRegistrationParameter>();
        //    response.IsSuccess = true;
        //    response.Message = "Successful";

        //    try
        //    {
        //        if (_sqlConnection.State != System.Data.ConnectionState.Open)
        //        {
        //            await _sqlConnection.OpenAsync();
        //        }

        //        if (request.File.FileName.ToLower().Contains(value: ".xlsx"))
        //        {
        //            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //            IExcelDataReader reader = ExcelReaderFactory.CreateReader(fileStream);
        //            DataSet dataSet = reader.AsDataSet(
        //                new ExcelDataSetConfiguration()
        //                {
        //                    UseColumnDataType = false,
        //                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
        //                    {
        //                        UseHeaderRow = true
        //                    }
        //                });

        //            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //            {
        //                StudentRegistrationParameter rows = new StudentRegistrationParameter();

        //                rows.Code = dataSet.Tables[0].Rows[i].ItemArray[0] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : null;
        //                rows.StudentFirstName = dataSet.Tables[0].Rows[i].ItemArray[1] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : null;
        //                rows.StudentLastName = dataSet.Tables[0].Rows[i].ItemArray[2] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[2]) : null;
        //                rows.StudentMiddleName = dataSet.Tables[0].Rows[i].ItemArray[3] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[3]) : null;
        //                rows.StandardName = dataSet.Tables[0].Rows[i].ItemArray[4] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[4]) : null;
        //                rows.SectionName = dataSet.Tables[0].Rows[i].ItemArray[5] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[5]) : null;
        //                rows.StudentDob = dataSet.Tables[0].Rows[i].ItemArray[6] != DBNull.Value ? (DateTime?)dataSet.Tables[0].Rows[i].ItemArray[6] : null;
        //                rows.StudentGenderId = dataSet.Tables[0].Rows[i].ItemArray[7] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[7]) : (int?)null;
        //                rows.StudentNic = dataSet.Tables[0].Rows[i].ItemArray[8] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[8]) : null;
        //                rows.StudentEmail = dataSet.Tables[0].Rows[i].ItemArray[9] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[9]) : null;
        //                rows.StudentIsHieduuser = dataSet.Tables[0].Rows[i].ItemArray[10] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[10]) : (int?)null;
        //                rows.StudentHieduActive = dataSet.Tables[0].Rows[i].ItemArray[11] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[11]) : (int?)null;
        //                rows.MotherFirstName = dataSet.Tables[0].Rows[i].ItemArray[12] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[12]) : null;
        //                rows.MotherLastName = dataSet.Tables[0].Rows[i].ItemArray[13] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[13]) : null;
        //                rows.MotherMiddleName = dataSet.Tables[0].Rows[i].ItemArray[14] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[14]) : null;
        //                rows.MotherPhoneNumber = dataSet.Tables[0].Rows[i].ItemArray[15] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[15]) : null;
        //                rows.MotherGenderId = dataSet.Tables[0].Rows[i].ItemArray[16] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[16]) : (int?)null;
        //                rows.MotherEmail = dataSet.Tables[0].Rows[i].ItemArray[17] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[17]) : null;
        //                rows.MotherIssmsuser = dataSet.Tables[0].Rows[i].ItemArray[18] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[18]) : (int?)null;
        //                rows.MotherIshigheduser = dataSet.Tables[0].Rows[i].ItemArray[19] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[19]) : (int?)null;
        //                rows.FatherFirstName = dataSet.Tables[0].Rows[i].ItemArray[20] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[20]) : null;
        //                rows.FatherLastName = dataSet.Tables[0].Rows[i].ItemArray[21] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[21]) : null;
        //                rows.FatherMiddleName = dataSet.Tables[0].Rows[i].ItemArray[22] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[22]) : null;
        //                rows.FatherPhoneNumber = dataSet.Tables[0].Rows[i].ItemArray[23] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[23]) : null;
        //                rows.FatherGenderId = dataSet.Tables[0].Rows[i].ItemArray[24] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[24]) : (int?)null;
        //                rows.FatherEmail = dataSet.Tables[0].Rows[i].ItemArray[25] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[25]) : null;
        //                rows.FatherIssmsuser = dataSet.Tables[0].Rows[i].ItemArray[26] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[26]) : (int?)null;
        //                rows.FatherIshigheduser = dataSet.Tables[0].Rows[i].ItemArray[27] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[27]) : (int?)null;
        //                Parameters.Add(rows);
        //            }



        //            if (Parameters.Count > 0)
        //            {
        //                string SqlQuery = "sp_StudentRegisterBulkUploderSheet";

        //                foreach (StudentRegistrationParameter rows in Parameters)
        //                {
        //                    StudentUploadStatus uploadStatus = new StudentUploadStatus
        //                    {
        //                        Code = rows.Code,
        //                        StudentFirstName = rows.StudentFirstName,
        //                        StudentLastName = rows.StudentLastName,
        //                        IsSuccess = true,
        //                        Message = "Uploaded successfully"
        //                    };

        //                    try
        //                    {
        //                        using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
        //                        {
        //                            sqlCommand.CommandType = CommandType.StoredProcedure;
        //                            sqlCommand.CommandTimeout = 180;
        //                            sqlCommand.Parameters.AddWithValue("@Code", rows.Code);
        //                            sqlCommand.Parameters.AddWithValue("@StudentFirstName", rows.StudentFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentLastName", rows.StudentLastName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentMiddleName", rows.StudentMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@StandardName", rows.StandardName);
        //                            sqlCommand.Parameters.AddWithValue("@SectionName", rows.SectionName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentDob", rows.StudentDob);
        //                            sqlCommand.Parameters.AddWithValue("@StudentGenderId", rows.StudentGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@StudentNic", rows.StudentNic);
        //                            sqlCommand.Parameters.AddWithValue("@StudentEmail", rows.StudentEmail);
        //                            sqlCommand.Parameters.AddWithValue("@StudentIsHieduuser", rows.StudentIsHieduuser);
        //                            sqlCommand.Parameters.AddWithValue("@StudentHieduActive", rows.StudentHieduActive);
        //                            sqlCommand.Parameters.AddWithValue("@MotherFirstName", rows.MotherFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherLastName", rows.MotherLastName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherMiddleName", rows.MotherMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherPhoneNumber", rows.MotherPhoneNumber);
        //                            sqlCommand.Parameters.AddWithValue("@MotherGenderId", rows.MotherGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@MotherEmail", rows.MotherEmail);
        //                            sqlCommand.Parameters.AddWithValue("@MotherIssmsuser", rows.MotherIssmsuser);
        //                            sqlCommand.Parameters.AddWithValue("@MotherIshigheduser", rows.MotherIshigheduser);
        //                            sqlCommand.Parameters.AddWithValue("@FatherFirstName", rows.FatherFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherLastName", rows.FatherLastName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherMiddleName", rows.FatherMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherPhoneNumber", rows.FatherPhoneNumber);
        //                            sqlCommand.Parameters.AddWithValue("@FatherGenderId", rows.FatherGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@FatherEmail", rows.FatherEmail);
        //                            sqlCommand.Parameters.AddWithValue("@FatherIssmsuser", rows.FatherIssmsuser);
        //                            sqlCommand.Parameters.AddWithValue("@FatherIshigheduser", rows.FatherIshigheduser);
        //                            sqlCommand.Parameters.AddWithValue("@SchoolId", request.SchoolId);
        //                            sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);

        //                            int Status = await sqlCommand.ExecuteNonQueryAsync();
        //                            if (Status <= 0)
        //                            {
        //                                uploadStatus.IsSuccess = false;
        //                                uploadStatus.Message = "Query Not Executed";
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        uploadStatus.IsSuccess = false;
        //                        uploadStatus.Message = ex.Message;
        //                    }

        //                    response.UploadStatuses.Add(uploadStatus);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Incorrect File";
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (_sqlConnection.State == System.Data.ConnectionState.Open)
        //        {
        //            await _sqlConnection.CloseAsync();
        //            await _sqlConnection.DisposeAsync();
        //        }
        //    }
        //    return response;
        //}


        ////CSV
        //public async Task<UploadCSVFileResponse> StudentRegisterUploadCSVFile(StudentRegistrationUploadCSVFileRequest request, Stream fileStream)
        //{
        //    UploadCSVFileResponse response = new UploadCSVFileResponse();
        //    List<StudentRegistrationParameter> Parameters = new List<StudentRegistrationParameter>();
        //    response.IsSuccess = true;
        //    response.Message = "Successful";

        //    try
        //    {
        //        if (request.File.FileName.ToLower().Contains(".csv"))
        //        {
        //            DataTable value = new DataTable();

        //            using (var csvReader = new CsvReader(new StreamReader(fileStream), true))
        //            {
        //                value.Load(csvReader);
        //            }

        //            for (int i = 0; i < value.Rows.Count; i++)
        //            {
        //                StudentRegistrationParameter rows = new StudentRegistrationParameter();

        //                rows.Code = value.Rows[i][0] != DBNull.Value ? Convert.ToString(value.Rows[i][0]) : null;
        //                rows.StudentFirstName = value.Rows[i][1] != DBNull.Value ? Convert.ToString(value.Rows[i][1]) : null;
        //                rows.StudentLastName = value.Rows[i][2] != DBNull.Value ? Convert.ToString(value.Rows[i][2]) : null;
        //                rows.StudentMiddleName = value.Rows[i][3] != DBNull.Value ? Convert.ToString(value.Rows[i][3]) : null;
        //                rows.StandardName = value.Rows[i][4] != DBNull.Value ? Convert.ToString(value.Rows[i][4]) : null;
        //                rows.SectionName = value.Rows[i][5] != DBNull.Value ? Convert.ToString(value.Rows[i][5]) : null;
        //                rows.StudentDob = value.Rows[i][6] != DBNull.Value ? (DateTime?)Convert.ToDateTime(value.Rows[i][6]) : null;
        //                rows.StudentGenderId = value.Rows[i][7] != DBNull.Value ? Convert.ToInt32(value.Rows[i][7]) : (int?)null;
        //                rows.StudentNic = value.Rows[i][8] != DBNull.Value ? Convert.ToString(value.Rows[i][8]) : null;
        //                rows.StudentEmail = value.Rows[i][9] != DBNull.Value ? Convert.ToString(value.Rows[i][9]) : null;
        //                rows.StudentIsHieduuser = value.Rows[i][10] != DBNull.Value ? Convert.ToInt32(value.Rows[i][10]) : (int?)null;
        //                rows.StudentHieduActive = value.Rows[i][11] != DBNull.Value ? Convert.ToInt32(value.Rows[i][11]) : (int?)null;
        //                rows.MotherFirstName = value.Rows[i][12] != DBNull.Value ? Convert.ToString(value.Rows[i][12]) : null;
        //                rows.MotherLastName = value.Rows[i][13] != DBNull.Value ? Convert.ToString(value.Rows[i][13]) : null;
        //                rows.MotherMiddleName = value.Rows[i][14] != DBNull.Value ? Convert.ToString(value.Rows[i][14]) : null;
        //                rows.MotherPhoneNumber = value.Rows[i][15] != DBNull.Value ? Convert.ToString(value.Rows[i][15]) : null;
        //                rows.MotherGenderId = value.Rows[i][16] != DBNull.Value ? Convert.ToInt32(value.Rows[i][16]) : (int?)null;
        //                rows.MotherEmail = value.Rows[i][17] != DBNull.Value ? Convert.ToString(value.Rows[i][17]) : null;
        //                rows.MotherIssmsuser = value.Rows[i][18] != DBNull.Value ? Convert.ToInt32(value.Rows[i][18]) : (int?)null;
        //                rows.MotherIshigheduser = value.Rows[i][19] != DBNull.Value ? Convert.ToInt32(value.Rows[i][19]) : (int?)null;
        //                rows.FatherFirstName = value.Rows[i][20] != DBNull.Value ? Convert.ToString(value.Rows[i][20]) : null;
        //                rows.FatherLastName = value.Rows[i][21] != DBNull.Value ? Convert.ToString(value.Rows[i][21]) : null;
        //                rows.FatherMiddleName = value.Rows[i][22] != DBNull.Value ? Convert.ToString(value.Rows[i][22]) : null;
        //                rows.FatherPhoneNumber = value.Rows[i][23] != DBNull.Value ? Convert.ToString(value.Rows[i][23]) : null;
        //                rows.FatherGenderId = value.Rows[i][24] != DBNull.Value ? Convert.ToInt32(value.Rows[i][24]) : (int?)null;
        //                rows.FatherEmail = value.Rows[i][25] != DBNull.Value ? Convert.ToString(value.Rows[i][25]) : null;
        //                rows.FatherIssmsuser = value.Rows[i][26] != DBNull.Value ? Convert.ToInt32(value.Rows[i][26]) : (int?)null;
        //                rows.FatherIshigheduser = value.Rows[i][27] != DBNull.Value ? Convert.ToInt32(value.Rows[i][27]) : (int?)null;

        //                Parameters.Add(rows);
        //            }




        //            if (Parameters.Count > 0)
        //            {
        //                string SqlQuery = "sp_StudentRegisterBulkUploderSheet";

        //                foreach (StudentRegistrationParameter rows in Parameters)
        //                {
        //                    StudentUploadStatus uploadStatus = new StudentUploadStatus
        //                    {
        //                        Code = rows.Code,
        //                        StudentFirstName = rows.StudentFirstName,
        //                        StudentLastName = rows.StudentLastName,
        //                        IsSuccess = true,
        //                        Message = "Uploaded successfully"
        //                    };

        //                    try
        //                    {
        //                        using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
        //                        {
        //                            sqlCommand.CommandType = CommandType.StoredProcedure;
        //                            sqlCommand.CommandTimeout = 180;

        //                            sqlCommand.Parameters.AddWithValue("@Code", rows.Code);
        //                            sqlCommand.Parameters.AddWithValue("@StudentFirstName", rows.StudentFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentLastName", rows.StudentLastName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentMiddleName", rows.StudentMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@StandardName", rows.StandardName);
        //                            sqlCommand.Parameters.AddWithValue("@SectionName", rows.SectionName);

        //                            sqlCommand.Parameters.AddWithValue("@StudentDob", rows.StudentDob);
        //                            sqlCommand.Parameters.AddWithValue("@StudentGenderId", rows.StudentGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@StudentNic", rows.StudentNic);
        //                            sqlCommand.Parameters.AddWithValue("@StudentEmail", rows.StudentEmail);
        //                            sqlCommand.Parameters.AddWithValue("@StudentIsHieduuser", rows.StudentIsHieduuser);
        //                            sqlCommand.Parameters.AddWithValue("@StudentHieduActive", rows.StudentHieduActive);

        //                            sqlCommand.Parameters.AddWithValue("@MotherFirstName", rows.MotherFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherLastName", rows.MotherLastName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherMiddleName", rows.MotherMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherPhoneNumber", rows.MotherPhoneNumber);
        //                            sqlCommand.Parameters.AddWithValue("@MotherGenderId", rows.MotherGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@MotherIssmsuser", rows.MotherIssmsuser);
        //                            sqlCommand.Parameters.AddWithValue("@MotherEmail", rows.MotherEmail);
        //                            sqlCommand.Parameters.AddWithValue("@MotherIshigheduser", rows.MotherIshigheduser);

        //                            sqlCommand.Parameters.AddWithValue("@FatherFirstName", rows.FatherFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherLastName", rows.FatherLastName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherMiddleName", rows.FatherMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherPhoneNumber", rows.FatherPhoneNumber);
        //                            sqlCommand.Parameters.AddWithValue("@FatherGenderId", rows.FatherGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@FatherEmail", rows.FatherEmail);
        //                            sqlCommand.Parameters.AddWithValue("@FatherIssmsuser", rows.FatherIssmsuser);
        //                            sqlCommand.Parameters.AddWithValue("@FatherIshigheduser", rows.FatherIshigheduser);


        //                            sqlCommand.Parameters.AddWithValue("@SchoolId", request.SchoolId);
        //                            sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);



        //                            int Status = await sqlCommand.ExecuteNonQueryAsync();
        //                            if (Status <= 0)
        //                            {
        //                                uploadStatus.IsSuccess = false;
        //                                uploadStatus.Message = "Query Not Executed";
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        uploadStatus.IsSuccess = false;
        //                        uploadStatus.Message = ex.Message;
        //                    }

        //                    response.UploadStatuses.Add(uploadStatus);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Incorrect File";
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (_sqlConnection.State == System.Data.ConnectionState.Open)
        //        {
        //            await _sqlConnection.CloseAsync();
        //            await _sqlConnection.DisposeAsync();
        //        }
        //    }
        //    return response;
        //}




        //Student Register Bulk Uploder Jaliya 07/09/2023, 24/7/2024

        //public async Task<UploadXMLFileResponse> StudentRegisterUploadXMLFile(StudentRegistrationUploadXMLFileRequest request, Stream fileStream)
        //{
        //    UploadXMLFileResponse response = new UploadXMLFileResponse();
        //    List<StudentRegistrationParameter> Parameters = new List<StudentRegistrationParameter>();
        //    response.IsSuccess = true;
        //    response.Message = "Successful";

        //    try
        //    {
        //        if (_sqlConnection.State != System.Data.ConnectionState.Open)
        //        {
        //            await _sqlConnection.OpenAsync();
        //        }

        //        if (request.File.FileName.ToLower().Contains(value: ".xlsx"))
        //        {
        //            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //            IExcelDataReader reader = ExcelReaderFactory.CreateReader(fileStream);
        //            DataSet dataSet = reader.AsDataSet(
        //                new ExcelDataSetConfiguration()
        //                {
        //                    UseColumnDataType = false,
        //                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
        //                    {
        //                        UseHeaderRow = true
        //                    }
        //                });

        //            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //            {
        //                StudentRegistrationParameter rows = new StudentRegistrationParameter();

        //                rows.Code = dataSet.Tables[0].Rows[i].ItemArray[0] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : null;
        //                rows.StudentFirstName = dataSet.Tables[0].Rows[i].ItemArray[1] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : null;
        //                rows.StudentLastName = dataSet.Tables[0].Rows[i].ItemArray[2] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[2]) : null;
        //                rows.StudentMiddleName = dataSet.Tables[0].Rows[i].ItemArray[3] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[3]) : null;
        //                rows.StudentDob = dataSet.Tables[0].Rows[i].ItemArray[4] != DBNull.Value ? (DateTime?)dataSet.Tables[0].Rows[i].ItemArray[4] : null;
        //                rows.StudentGenderId = (int)(dataSet.Tables[0].Rows[i].ItemArray[5] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[5]) : (int?)null);
        //                rows.StudentNic = dataSet.Tables[0].Rows[i].ItemArray[6] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[6]) : null;
        //                rows.StudentEmail = dataSet.Tables[0].Rows[i].ItemArray[7] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[7]) : null;
        //                rows.StudentIsHieduuser = dataSet.Tables[0].Rows[i].ItemArray[8] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[8]) : (int?)null;
        //                rows.StudentHieduActive = dataSet.Tables[0].Rows[i].ItemArray[9] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[9]) : (int?)null;
        //                rows.MotherFirstName = dataSet.Tables[0].Rows[i].ItemArray[10] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[10]) : null;
        //                rows.MotherLastName = dataSet.Tables[0].Rows[i].ItemArray[11] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[11]) : null;
        //                rows.MotherMiddleName = dataSet.Tables[0].Rows[i].ItemArray[12] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[12]) : null;
        //                rows.MotherPhoneNumber = dataSet.Tables[0].Rows[i].ItemArray[13] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[13]) : null;
        //                rows.MotherGenderId = dataSet.Tables[0].Rows[i].ItemArray[14] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[14]) : (int?)null;
        //                rows.MotherEmail = dataSet.Tables[0].Rows[i].ItemArray[15] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[15]) : null;
        //                rows.MotherIssmsuser = dataSet.Tables[0].Rows[i].ItemArray[16] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[16]) : (int?)null;
        //                rows.MotherIshigheduser = dataSet.Tables[0].Rows[i].ItemArray[17] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[17]) : (int?)null;
        //                rows.FatherFirstName = dataSet.Tables[0].Rows[i].ItemArray[18] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[18]) : null;
        //                rows.FatherLastName = dataSet.Tables[0].Rows[i].ItemArray[19] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[19]) : null;
        //                rows.FatherMiddleName = dataSet.Tables[0].Rows[i].ItemArray[20] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[20]) : null;
        //                rows.FatherPhoneNumber = dataSet.Tables[0].Rows[i].ItemArray[21] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[21]) : null;
        //                rows.FatherGenderId = dataSet.Tables[0].Rows[i].ItemArray[22] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[22]) : (int?)null;
        //                rows.FatherEmail = dataSet.Tables[0].Rows[i].ItemArray[23] != DBNull.Value ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[23]) : null;
        //                rows.FatherIssmsuser = dataSet.Tables[0].Rows[i].ItemArray[24] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[24]) : (int?)null;
        //                rows.FatherIshigheduser = dataSet.Tables[0].Rows[i].ItemArray[25] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[25]) : (int?)null;
        //                Parameters.Add(rows);
        //            }


        //            if (Parameters.Count > 0)
        //            {
        //                string SqlQuery = "sp_StudentRegisterBulkUploderSheet";

        //                foreach (StudentRegistrationParameter rows in Parameters)
        //                {
        //                    StudentUploadStatus uploadStatus = new StudentUploadStatus
        //                    {
        //                        Code = rows.Code,
        //                        StudentFirstName = rows.StudentFirstName,
        //                        StudentLastName = rows.StudentLastName,
        //                        IsSuccess = true,
        //                        Message = "Uploaded successfully"
        //                    };

        //                    try
        //                    {
        //                        using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
        //                        {
        //                            sqlCommand.CommandType = CommandType.StoredProcedure;
        //                            sqlCommand.CommandTimeout = 180;
        //                            sqlCommand.Parameters.AddWithValue("@Code", rows.Code);
        //                            sqlCommand.Parameters.AddWithValue("@StudentFirstName", rows.StudentFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentLastName", rows.StudentLastName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentMiddleName", rows.StudentMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentDob", rows.StudentDob);
        //                            sqlCommand.Parameters.AddWithValue("@StudentGenderId", rows.StudentGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@StudentNic", rows.StudentNic);
        //                            sqlCommand.Parameters.AddWithValue("@StudentEmail", rows.StudentEmail);
        //                            sqlCommand.Parameters.AddWithValue("@StudentIsHieduuser", rows.StudentIsHieduuser);
        //                            sqlCommand.Parameters.AddWithValue("@StudentHieduActive", rows.StudentHieduActive);
        //                            sqlCommand.Parameters.AddWithValue("@MotherFirstName", rows.MotherFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherLastName", rows.MotherLastName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherMiddleName", rows.MotherMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherPhoneNumber", rows.MotherPhoneNumber);
        //                            sqlCommand.Parameters.AddWithValue("@MotherGenderId", rows.MotherGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@MotherEmail", rows.MotherEmail);
        //                            sqlCommand.Parameters.AddWithValue("@MotherIssmsuser", rows.MotherIssmsuser);
        //                            sqlCommand.Parameters.AddWithValue("@MotherIshigheduser", rows.MotherIshigheduser);
        //                            sqlCommand.Parameters.AddWithValue("@FatherFirstName", rows.FatherFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherLastName", rows.FatherLastName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherMiddleName", rows.FatherMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherPhoneNumber", rows.FatherPhoneNumber);
        //                            sqlCommand.Parameters.AddWithValue("@FatherGenderId", rows.FatherGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@FatherEmail", rows.FatherEmail);
        //                            sqlCommand.Parameters.AddWithValue("@FatherIssmsuser", rows.FatherIssmsuser);
        //                            sqlCommand.Parameters.AddWithValue("@FatherIshigheduser", rows.FatherIshigheduser);
        //                            sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);
        //                            sqlCommand.Parameters.AddWithValue("@LevelId", request.LevelId);
        //                            sqlCommand.Parameters.AddWithValue("@StandardId", request.StandardId);
        //                            sqlCommand.Parameters.AddWithValue("@SectionId", request.SectionId);

        //                            int Status = await sqlCommand.ExecuteNonQueryAsync();
        //                            if (Status <= 0)
        //                            {
        //                                uploadStatus.IsSuccess = false;
        //                                uploadStatus.Message = "Query Not Executed";
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        uploadStatus.IsSuccess = false;
        //                        uploadStatus.Message = ex.Message;
        //                    }

        //                    response.UploadStatuses.Add(uploadStatus);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Incorrect File";
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (_sqlConnection.State == System.Data.ConnectionState.Open)
        //        {
        //            await _sqlConnection.CloseAsync();
        //            await _sqlConnection.DisposeAsync();
        //        }
        //    }
        //    return response;
        //}




        //Student Register Bulk Uploder Jaliya 07/09/2023, 24/7/2024
        //public async Task<UploadCSVFileResponse> StudentRegisterUploadCSVFile(StudentRegistrationUploadCSVFileRequest request, Stream fileStream)
        //{
        //    UploadCSVFileResponse response = new UploadCSVFileResponse();
        //    List<StudentRegistrationParameter> Parameters = new List<StudentRegistrationParameter>();
        //    response.IsSuccess = true;
        //    response.Message = "Successful";

        //    try
        //    {
        //        if (request.File.FileName.ToLower().Contains(".csv"))
        //        {
        //            DataTable value = new DataTable();

        //            using (var csvReader = new CsvReader(new StreamReader(fileStream), true))
        //            {
        //                value.Load(csvReader);
        //            }

        //            for (int i = 0; i < value.Rows.Count; i++)
        //            {
        //                StudentRegistrationParameter rows = new StudentRegistrationParameter();

        //                rows.Code = value.Rows[i][0] != DBNull.Value ? Convert.ToString(value.Rows[i][0]) : null;
        //                rows.StudentFirstName = value.Rows[i][1] != DBNull.Value ? Convert.ToString(value.Rows[i][1]) : null;
        //                rows.StudentLastName = value.Rows[i][2] != DBNull.Value ? Convert.ToString(value.Rows[i][2]) : null;
        //                rows.StudentMiddleName = value.Rows[i][3] != DBNull.Value ? Convert.ToString(value.Rows[i][3]) : null;
        //                rows.StudentDob = value.Rows[i][4] != DBNull.Value ? (DateTime?)Convert.ToDateTime(value.Rows[i][4]) : null;
        //                rows.StudentGenderId = value.Rows[i][5] != DBNull.Value ? Convert.ToInt32(value.Rows[i][5]) : (int?)null;
        //                rows.StudentNic = value.Rows[i][6] != DBNull.Value ? Convert.ToString(value.Rows[i][6]) : null;
        //                rows.StudentEmail = value.Rows[i][7] != DBNull.Value ? Convert.ToString(value.Rows[i][7]) : null;
        //                rows.StudentIsHieduuser = value.Rows[i][8] != DBNull.Value ? Convert.ToInt32(value.Rows[i][8]) : (int?)null;
        //                rows.StudentHieduActive = value.Rows[i][9] != DBNull.Value ? Convert.ToInt32(value.Rows[i][9]) : (int?)null;
        //                rows.MotherFirstName = value.Rows[i][10] != DBNull.Value ? Convert.ToString(value.Rows[i][10]) : null;
        //                rows.MotherLastName = value.Rows[i][11] != DBNull.Value ? Convert.ToString(value.Rows[i][11]) : null;
        //                rows.MotherMiddleName = value.Rows[i][12] != DBNull.Value ? Convert.ToString(value.Rows[i][12]) : null;
        //                rows.MotherPhoneNumber = value.Rows[i][13] != DBNull.Value ? Convert.ToString(value.Rows[i][13]) : null;
        //                rows.MotherGenderId = value.Rows[i][14] != DBNull.Value ? Convert.ToInt32(value.Rows[i][14]) : (int?)null;
        //                rows.MotherEmail = value.Rows[i][15] != DBNull.Value ? Convert.ToString(value.Rows[i][15]) : null;
        //                rows.MotherIssmsuser = value.Rows[i][16] != DBNull.Value ? Convert.ToInt32(value.Rows[i][16]) : (int?)null;
        //                rows.MotherIshigheduser = value.Rows[i][17] != DBNull.Value ? Convert.ToInt32(value.Rows[i][17]) : (int?)null;
        //                rows.FatherFirstName = value.Rows[i][18] != DBNull.Value ? Convert.ToString(value.Rows[i][18]) : null;
        //                rows.FatherLastName = value.Rows[i][19] != DBNull.Value ? Convert.ToString(value.Rows[i][19]) : null;
        //                rows.FatherMiddleName = value.Rows[i][20] != DBNull.Value ? Convert.ToString(value.Rows[i][20]) : null;
        //                rows.FatherPhoneNumber = value.Rows[i][21] != DBNull.Value ? Convert.ToString(value.Rows[i][21]) : null;
        //                rows.FatherGenderId = value.Rows[i][22] != DBNull.Value ? Convert.ToInt32(value.Rows[i][22]) : (int?)null;
        //                rows.FatherEmail = value.Rows[i][23] != DBNull.Value ? Convert.ToString(value.Rows[i][23]) : null;
        //                rows.FatherIssmsuser = value.Rows[i][24] != DBNull.Value ? Convert.ToInt32(value.Rows[i][24]) : (int?)null;
        //                rows.FatherIshigheduser = value.Rows[i][25] != DBNull.Value ? Convert.ToInt32(value.Rows[i][25]) : (int?)null;

        //                Parameters.Add(rows);
        //            }




        //            if (Parameters.Count > 0)
        //            {
        //                string SqlQuery = "sp_StudentRegisterBulkUploderSheet";

        //                foreach (StudentRegistrationParameter rows in Parameters)
        //                {
        //                    StudentUploadStatus uploadStatus = new StudentUploadStatus
        //                    {
        //                        Code = rows.Code,
        //                        StudentFirstName = rows.StudentFirstName,
        //                        StudentLastName = rows.StudentLastName,
        //                        IsSuccess = true,
        //                        Message = "Uploaded successfully"
        //                    };

        //                    try
        //                    {
        //                        using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
        //                        {
        //                            sqlCommand.CommandType = CommandType.StoredProcedure;
        //                            sqlCommand.CommandTimeout = 180;

        //                            sqlCommand.Parameters.AddWithValue("@Code", rows.Code);
        //                            sqlCommand.Parameters.AddWithValue("@StudentFirstName", rows.StudentFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentLastName", rows.StudentLastName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentMiddleName", rows.StudentMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@StudentDob", rows.StudentDob);
        //                            sqlCommand.Parameters.AddWithValue("@StudentGenderId", rows.StudentGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@StudentNic", rows.StudentNic);
        //                            sqlCommand.Parameters.AddWithValue("@StudentEmail", rows.StudentEmail);
        //                            sqlCommand.Parameters.AddWithValue("@StudentIsHieduuser", rows.StudentIsHieduuser);
        //                            sqlCommand.Parameters.AddWithValue("@StudentHieduActive", rows.StudentHieduActive);

        //                            sqlCommand.Parameters.AddWithValue("@MotherFirstName", rows.MotherFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherLastName", rows.MotherLastName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherMiddleName", rows.MotherMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@MotherPhoneNumber", rows.MotherPhoneNumber);
        //                            sqlCommand.Parameters.AddWithValue("@MotherGenderId", rows.MotherGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@MotherIssmsuser", rows.MotherIssmsuser);
        //                            sqlCommand.Parameters.AddWithValue("@MotherEmail", rows.MotherEmail);
        //                            sqlCommand.Parameters.AddWithValue("@MotherIshigheduser", rows.MotherIshigheduser);

        //                            sqlCommand.Parameters.AddWithValue("@FatherFirstName", rows.FatherFirstName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherLastName", rows.FatherLastName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherMiddleName", rows.FatherMiddleName);
        //                            sqlCommand.Parameters.AddWithValue("@FatherPhoneNumber", rows.FatherPhoneNumber);
        //                            sqlCommand.Parameters.AddWithValue("@FatherGenderId", rows.FatherGenderId);
        //                            sqlCommand.Parameters.AddWithValue("@FatherEmail", rows.FatherEmail);
        //                            sqlCommand.Parameters.AddWithValue("@FatherIssmsuser", rows.FatherIssmsuser);
        //                            sqlCommand.Parameters.AddWithValue("@FatherIshigheduser", rows.FatherIshigheduser);


        //                            sqlCommand.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);
        //                            sqlCommand.Parameters.AddWithValue("@LevelId", request.LevelId);
        //                            sqlCommand.Parameters.AddWithValue("@StandardId", request.StandardId);
        //                            sqlCommand.Parameters.AddWithValue("@SectionId", request.SectionId);

        //                            int Status = await sqlCommand.ExecuteNonQueryAsync();
        //                            if (Status <= 0)
        //                            {
        //                                uploadStatus.IsSuccess = false;
        //                                uploadStatus.Message = "Query Not Executed";
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        uploadStatus.IsSuccess = false;
        //                        uploadStatus.Message = ex.Message;
        //                    }

        //                    response.UploadStatuses.Add(uploadStatus);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Incorrect File";
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (_sqlConnection.State == System.Data.ConnectionState.Open)
        //        {
        //            await _sqlConnection.CloseAsync();
        //            await _sqlConnection.DisposeAsync();
        //        }
        //    }
        //    return response;
        //}






    }
}
