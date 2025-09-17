using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        //Hardcoded for now. Needs to be discussed on the DB side and assigned a value.
        readonly short DeleteStatusID = 3;

        private readonly IMChildinfoService mChildInfoService;
        private readonly IMBranchService mBranchService;
        private readonly IMStandardsectionmappingService mStandardsectionmapping;
        private readonly IMBusinessUnitTypeService mBusinessUnitTypeService;
        private readonly IMGenderService mGenderService;
        private readonly IMLocationtypeService mLocationtypeService;
        private readonly IMModuleService mModuleService;
        private readonly IMRelationtypeService mRelationtypeService;
        private readonly IMSchoolService mSchoolService;
        private readonly IMStatusService mStatusService;
        private readonly IMStatustypeService mStatustypeService;
        private readonly IMLocationService mLocationService;
        private readonly IMChildschoolmappingService mChildschoolmappingService;
        private readonly IMParentchildmappingService mParentchildmappingService;
        private readonly IMUsermodulemappingService mUsermodulemappingService;
        private readonly IAppuserdeviceService AppuserdeviceService;
        private readonly IMFeatureService mFeatureService;

        public MasterController(
            IMChildinfoService _mChildInfoService,
            IMBranchService _mBranchService,
            IMStandardsectionmappingService _mStandardsectionmapping,
            IMBusinessUnitTypeService _mBusinessUnitTypeService,
            IMGenderService _mGenderService,
            IMLocationtypeService _mLocationtypeService,
            IMModuleService _mModuleService,
            IMRelationtypeService _mRelationtypeService,
            IMSchoolService _mSchoolService,
            IMStatusService _mStatusService,
            IMStatustypeService _mStatustypeService,
            IMLocationService _mLocationService,
            IMChildschoolmappingService _mChildschoolmappingService,
            IMParentchildmappingService _mParentchildmappingService,
            IMUsermodulemappingService _mUsermodulemappingService,
            IAppuserdeviceService _appUserDeviceService,
            IMFeatureService _mFeatureService
            )
        {
            mChildInfoService = _mChildInfoService;
            mBranchService = _mBranchService;
            mStandardsectionmapping = _mStandardsectionmapping;
            mBusinessUnitTypeService = _mBusinessUnitTypeService;
            mGenderService = _mGenderService;
            mLocationtypeService = _mLocationtypeService;
            mModuleService = _mModuleService;
            mRelationtypeService = _mRelationtypeService;
            mSchoolService = _mSchoolService;
            mStatusService = _mStatusService;
            mStatustypeService = _mStatustypeService;
            mLocationService = _mLocationService;
            mChildschoolmappingService = _mChildschoolmappingService;
            mParentchildmappingService = _mParentchildmappingService;
            mUsermodulemappingService = _mUsermodulemappingService;
            AppuserdeviceService = _appUserDeviceService;
            mFeatureService = _mFeatureService;
        }


        //[Route("getstd")]
        //[HttpGet]
        //public async Task<object> getstd(int a)
        //{
        //    try
        //    {
        //        return await mStandardsectionmapping.GetEntityByBranchID2(a);
        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }

        //}



        #region MBranch

        [Route("AddMBranch")]
        [HttpPost]
        public async Task<IActionResult> AddMBranch([FromBody] MBranchModel model)
        {
            try
            {
                var temp = await this.mBranchService.GetEntityByName(model.Name);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MBranch already Exists."
                    });
                }
                var id = await this.mBranchService.AddEntity(new MBranch
                {
                    Name = model.Name,
                    Description = model.Description,
                    Code = model.Code,
                    Principalname = model.Principalname,
                    Address = model.Address,
                    Pincode = model.Pincode,
                    Locaionid = model.Locaionid,
                    Schoolid = model.Schoolid,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMBranchByID/{MBranchID}")]
        [HttpGet]

        public async Task<IActionResult> getMBranchByID(int MBranchID)
        {
            try
            {
                var temp = await this.mBranchService.GetEntityByID(MBranchID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MBranch doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MBranch successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }

        }


        [Route("getMBranchByName/{MBranchName}")]
        [HttpGet]

        public async Task<IActionResult> getMBranchByName(string MBranchName)
        {
            try
            {
                var temp = await this.mBranchService.GetEntityByName(MBranchName);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MBranch doesn't exist with this name.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    Message = "MBranch successfully fetched.",
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMBranches")]
        [HttpGet]
        public async Task<IActionResult> getAllMBranches()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mBranchService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MBranches successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMBranch")]
        [HttpPost]

        public async Task<IActionResult> UpdateMBranch([FromBody] MBranchUpdateModel model)
        {
            try
            {
                var temp = await this.mBranchService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MBranch doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Code))
                    temp.Code = model.Code;
                if (!string.IsNullOrEmpty(model.Description))
                    temp.Description = model.Description;
                if (model.Schoolid.HasValue)
                    temp.Schoolid = model.Schoolid;
                if (!string.IsNullOrEmpty(model.Principalname))
                    temp.Principalname = model.Principalname;
                if (!string.IsNullOrEmpty(model.Address))
                    temp.Address = model.Address;
                if (model.Locaionid.HasValue)
                    temp.Locaionid = model.Locaionid;
                if (model.Pincode.HasValue)
                    temp.Pincode = model.Pincode;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mBranchService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MBranch updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        #endregion


        #region MStandardsectionmapping

        [Route("AddMStandardsectionmapping")]
        [HttpPost]
        public async Task<IActionResult> AddMStandardsectionmapping([FromBody] MStandardsectionmappingModel model)
        {
            try
            {
                var temp = await this.mStandardsectionmapping.GetEntityByName(model.Name);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MStandardsectionmapping already exists."
                    });
                }
                var id = await this.mStandardsectionmapping.AddEntity(new MStandardsectionmapping
                {
                    Name = model.Name,
                    Description = model.Description,
                    Createdby = model.Createdby,
                    Parentid = model.Parentid,
                    Branchid = model.Branchid,
                    Businessunittypeid = model.Businessunittypeid,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMStandardsectionmappingByID/{MStandardsectionmappingID}")]
        [HttpGet]

        public async Task<IActionResult> getMStandardsectionmappingByID(int MStandardsectionmappingID)
        {
            try
            {
                var temp = await this.mStandardsectionmapping.GetEntityByID(MStandardsectionmappingID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStandardsectionmapping doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStandardsectionmapping successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getMStandardsectionmappingByBranchID/{BranchId}")]
        [HttpGet]

        public async Task<IActionResult> getMStandardsectionmappingByBranchID(int BranchId)
        {
            try
            {
                var temp = await this.mStandardsectionmapping.GetEntityBySchoolID2(BranchId);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStandardsectionmapping doesn't exists with this BranchID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStandardsectionmapping successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        //sanduni LevelStandardSectionMapping
        [Route("GetLevelStandardSectionMapping")]
        [HttpGet]

        public async Task<IActionResult> GetLevelStandardSectionMapping(int SchoolId,int LevelId)
        {
            try
            {
                var temp = await this.mStandardsectionmapping.GetEntityByLevelSchoolID2(SchoolId, LevelId);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStandardsectionmapping doesn't exists with this SchoolID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(temp);

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }
        //[Route("GetLevelStandardSectionMappingSubjectTeacher")]
        //[HttpGet]

        //public async Task<IActionResult> GetLevelStandardSectionMappingSubjectTeacher(int SchoolId, int TeacherId)
        //{
        //    try
        //    {
        //        var temp = await this.mStandardsectionmapping.GetLevelStandardSectionMappingSubjectTeacher(SchoolId, TeacherId);
        //        if (temp == null)
        //        {
        //            return NotFound(new
        //            {
        //                Data = "MStandardsectionmapping doesn't exists with this SchoolID.",
        //                StatusCode = HttpStatusCode.NotFound
        //            });
        //        }
        //        return Ok(temp);

        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new
        //        {
        //            Data = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //        });
        //    }

        //}
        [Route("GetLevelStandardSectionMappingbyBrachId")]
        [HttpGet]
        public async Task<IActionResult> GetLevelStandardSectionMappingbyBrachId(int branchId)
        {
            try
            {
                var res = await this.mStandardsectionmapping.GetStandardSectionMappingByBranchIdAsync(branchId);
                if (res == null || res.Count == 0)
                {
                    return BadRequest("Records not found");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("GetStandardSectionMapping")]
        [HttpGet]

        public async Task<IActionResult> GetStandardSectionMapping(int SchoolId)
        {
            try
            {
                var temp = await this.mStandardsectionmapping.GetEntityBySchoolID2(SchoolId);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStandardsectionmapping doesn't exists with this SchoolID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(temp);

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getAllMStandardsectionmappings")]
        [HttpGet]
        public async Task<IActionResult> getAllMStandardsectionmappings()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mStandardsectionmapping.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStandardsectionmappings successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMStandardsectionmapping")]
        [HttpPost]

        public async Task<IActionResult> UpdateMStandardsectionmapping([FromBody] MStandardsectionmappingUpdateModel model)
        {
            try
            {
                var temp = await this.mStandardsectionmapping.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStandardsectionmapping doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Description))
                    temp.Description = model.Description;
                if (model.Parentid.HasValue)
                    temp.Parentid = model.Parentid;
                if (model.Branchid.HasValue)
                    temp.Branchid = model.Branchid;
                if (model.Businessunittypeid.HasValue)
                    temp.Businessunittypeid = model.Businessunittypeid;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mStandardsectionmapping.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStandardsectionmapping updated successfully."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        #endregion


        #region MBuisnessunittype

        [Route("AddMBuisnessUnitType")]
        [HttpPost]
        public async Task<IActionResult> AddMBuisnessUnitType([FromBody] MBuisnessunittypeModel model)
        {
            try
            {
                var temp = await this.mBusinessUnitTypeService.GetEntityByType(model.Type);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MBuisnessunittype already exists."
                    });
                }
                var id = await this.mBusinessUnitTypeService.AddEntity(new MBusinessunittype
                {
                    Type = model.Type,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMBuisnessUnitTypeByID/{MBuisnessunittypeID}")]
        [HttpGet]

        public async Task<IActionResult> getMBuisnessUnitTypeByID(int MBuisnessunittypeID)
        {
            try
            {
                var temp = await this.mBusinessUnitTypeService.GetEntityByID(MBuisnessunittypeID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MBuisnessunittype doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MBuisnessunittype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMBuisnessUnitTypeByType/{MBuisnessunitType}")]
        [HttpGet]

        public async Task<IActionResult> getMBuisnessUnitTypeByType(string MBuisnessunitType)
        {
            try
            {
                var temp = await this.mBusinessUnitTypeService.GetEntityByType(MBuisnessunitType);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MBuisnessunit doesn't exist with this type.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MBuisnessunittype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMBuisnessUnitTypes")]
        [HttpGet]
        public async Task<IActionResult> getAllMBuisnessUnitTypes()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mBusinessUnitTypeService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MBuisnessunittypes successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMBuisnessUnitType")]
        [HttpPost]

        public async Task<IActionResult> UpdateMBuisnessUnitType([FromBody] MBuisnessunittypeUpdateModel model)
        {
            try
            {
                var temp = await this.mBusinessUnitTypeService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MBuisnessunittype doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Type))
                    temp.Type = model.Type;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mBusinessUnitTypeService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MBuisnessunittype updated successfully."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        #endregion

        #region MChildInfo

        [Route("AddMChildInfo")]
        [HttpPost]
        public async Task<IActionResult> AddMChildInfo([FromBody] MChildInfoModel model)
        {
            try
            {
                var temp = await this.mChildInfoService.GetEntityByName(model.Firstname);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MChildInfo already Exists."
                    });
                }
                var id = await this.mChildInfoService.AddEntity(new MChildinfo
                {
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Middlename = model.Middlename,
                    Dob = model.Dob,
                    Phonenumber = model.Phonenumber,
                    Genderid = model.Genderid,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMChildInfoByID/{MChildInfoID}")]
        [HttpGet]

        public async Task<IActionResult> getMChildInfoByID(int MChildInfoID)
        {
            try
            {
                var temp = await this.mChildInfoService.GetEntityByID(MChildInfoID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MChildInfo doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MChildInfo successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMChildInfoByName/{MChildInfoName}")]
        [HttpGet]

        public async Task<IActionResult> getMChildInfoByName(string MChildInfoName)
        {
            try
            {
                var temp = await this.mChildInfoService.GetEntityByName(MChildInfoName);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MChildInfo doesn't exist with this name.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MChildInfo successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMChildInfos")]
        [HttpGet]
        public async Task<IActionResult> getAllMChildInfos()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mChildInfoService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MChildInfos successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMChildInfo")]
        [HttpPost]

        public async Task<IActionResult> UpdateMChildInfo([FromBody] MChildInfoUpdateModel model)
        {
            try
            {
                var temp = await this.mChildInfoService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MChildInfo doesn't exist with this id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                if (!string.IsNullOrEmpty(model.Firstname))
                    temp.Firstname = model.Firstname;
                if (!string.IsNullOrEmpty(model.Lastname))
                    temp.Lastname = model.Lastname;
                if (!string.IsNullOrEmpty(model.Middlename))
                    temp.Middlename = model.Middlename;
                if (model.Dob.HasValue)
                    temp.Dob = model.Dob;
                if (model.Genderid.HasValue)
                    temp.Genderid = model.Genderid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Createdby;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mChildInfoService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MChildInfo updated successfully."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        #endregion

        #region MGender

        [Route("AddMGender")]
        [HttpPost]
        public async Task<IActionResult> AddMGender([FromBody] MGenderModel model)
        {
            try
            {
                var temp = await this.mGenderService.GetEntityByType(model.Type);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MGender already exists."
                    });
                }
                var id = await this.mGenderService.AddEntity(new MGender
                {
                    Type = model.Type,
                    Icon = model.Icon,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMGenderByID/{MGenderID}")]
        [HttpGet]

        public async Task<IActionResult> getMGenderByID(int MGenderID)
        {
            try
            {
                var temp = await this.mGenderService.GetEntityByID(MGenderID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MGender doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MGender successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMGenderByType/{MGenderType}")]
        [HttpGet]

        public async Task<IActionResult> getMGenderByType(string MGenderType)
        {
            try
            {
                var temp = await this.mGenderService.GetEntityByType(MGenderType);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MGender doesn't exist with this type.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MGender successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMGenders")]
        [HttpGet]
        public async Task<IActionResult> getAllMGenders()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mGenderService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MGender successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMGender")]
        [HttpPost]

        public async Task<IActionResult> UpdateMGender([FromBody] MGenderUpdateModel model)
        {
            try
            {
                var temp = await this.mGenderService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MGender doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Type))
                    temp.Type = model.Type;
                if (!string.IsNullOrEmpty(model.Icon))
                    temp.Icon = model.Icon;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                if (model.Createdby > 0)
                    temp.Createdby = model.Createdby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mGenderService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MGender updated successfully."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        #endregion

        #region MLocation

        [Route("AddMLocation")]
        [HttpPost]
        public async Task<IActionResult> AddMLocation([FromBody] MLocationModel model)
        {
            try
            {
                var temp = await this.mLocationService.GetEntityByName(model.Name);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MLocation already exists."
                    });
                }
                var id = await this.mLocationService.AddEntity(new MLocation
                {
                    Name = model.Name,
                    Parentid = model.Parentid,
                    Locationtypeid = model.Locationtypeid,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMLocationByID/{MLocationID}")]
        [HttpGet]

        public async Task<IActionResult> getMLocationByID(int MLocationID)
        {
            try
            {
                var temp = await this.mLocationService.GetEntityByID(MLocationID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MLocation doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MLocation successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMLocationByName/{MLocationName}")]
        [HttpGet]

        public async Task<IActionResult> getMLocationByName(string MLocationName)
        {
            try
            {
                var temp = await this.mLocationService.GetEntityByName(MLocationName);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MLocation doesn't exist with this name.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MLocation successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMLocations")]
        [HttpGet]
        public async Task<IActionResult> getAllMLocations()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mLocationService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MLocations successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMLocation")]
        [HttpPost]

        public async Task<IActionResult> UpdateMLocation([FromBody] MLocationUpdateModel model)
        {
            try
            {
                var temp = await this.mLocationService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MLocation doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (model.Parentid.HasValue)
                    temp.Parentid = model.Parentid;
                if (model.Locationtypeid.HasValue)
                    temp.Locationtypeid = model.Locationtypeid;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mLocationService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MLocation updated successfully."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        #endregion

        #region MLocationtype

        [Route("AddMLocationType")]
        [HttpPost]
        public async Task<IActionResult> AddMLocationType([FromBody] MLocationtypeModel model)
        {
            try
            {
                var temp = await this.mLocationtypeService.GetEntityByType(model.Type);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MLocationtype already exists."
                    });
                }
                var id = await this.mLocationtypeService.AddEntity(new MLocationtype
                {
                    Type = model.Type,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMLocationTypeByID/{MLocationtypeID}")]
        [HttpGet]

        public async Task<IActionResult> getMLocationTypeByID(int MLocationtypeID)
        {
            try
            {
                var temp = await this.mLocationtypeService.GetEntityByID(MLocationtypeID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MLocationtype doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MLocationtype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMLocationTypeByType/{MLocationType}")]
        [HttpGet]

        public async Task<IActionResult> getMLocationTypeByType(string MLocationType)
        {
            try
            {
                var temp = await this.mLocationtypeService.GetEntityByType(MLocationType);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MLocationtype doesn't exist with this type.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MLocationtype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMLocationtypes")]
        [HttpGet]
        public async Task<IActionResult> getAllMLocationtypes()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mLocationtypeService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MLocationtypes successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMLocationtype")]
        [HttpPost]

        public async Task<IActionResult> UpdateMLocationtype([FromBody] MLocationtypeUpdateModel model)
        {
            try
            {
                var temp = await this.mLocationtypeService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MLocationtype doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Type))
                    temp.Type = model.Type;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mLocationtypeService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MLocationtype updated successfully."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        #endregion

        #region AddMModule

        [Route("AddMModule")]
        [HttpPost]
        public async Task<IActionResult> AddMModule([FromBody] MModuleModel model)
        {
            try
            {
                var temp = await this.mModuleService.GetEntityByName(model.Name);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MModule already Exists."
                    });
                }
                var id = await this.mModuleService.AddEntity(new MModule
                {
                    Name = model.Name,
                    State = model.State,
                    Icon = model.Icon,
                    Type = model.Type,
                    Selected = model.Selected,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                    Parentid = model.Parentid
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMModuleByID/{MModuleID}")]
        [HttpGet]

        public async Task<IActionResult> getMModuleByID(int MModuleID)
        {
            try
            {
                var temp = await this.mModuleService.GetEntityByID(MModuleID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MModule doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MModule successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMModuleByName/{MModuleName}")]
        [HttpGet]

        public async Task<IActionResult> getMModuleByName(string MModuleName)
        {
            try
            {
                var temp = await this.mModuleService.GetEntityByName(MModuleName);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MModule doesn't exist with this name.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    Message = "MModule successfully fetched.",
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMModule")]
        [HttpGet]
        public async Task<IActionResult> getAllMModule()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mModuleService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MModule successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMModule")]
        [HttpPost]

        public async Task<IActionResult> UpdateMModule([FromBody] MModuleUpdateModel model)
        {
            try
            {
                var temp = await this.mModuleService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MModule doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.State))
                    temp.State = model.State;
                if (!string.IsNullOrEmpty(model.Icon))
                    temp.Icon = model.Icon;
                if (!string.IsNullOrEmpty(model.Type))
                    temp.Type = model.Type;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Selected.HasValue)
                    temp.Selected = model.Selected;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Parentid.HasValue)
                    temp.Parentid = model.Parentid;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mModuleService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MModule updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        #endregion

        #region MRelationtype

        [Route("AddMRelationtype")]
        [HttpPost]
        public async Task<IActionResult> AddMRelationtype([FromBody] MRelationtypeModel model)
        {
            try
            {
                var temp = await this.mRelationtypeService.GetEntityByType(model.Type);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MRelationtype already exists."
                    });
                }
                var id = await this.mRelationtypeService.AddEntity(new MRelationtype
                {
                    Type = model.Type,
                    Icon = model.Icon,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMRelationTypeByID/{MRelationTypeID}")]
        [HttpGet]

        public async Task<IActionResult> getMRelationTypeByID(int MRelationtypeID)
        {
            try
            {
                var temp = await this.mRelationtypeService.GetEntityByID(MRelationtypeID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MRelationtype doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MRelationtype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMRelationTypeByType/{MRelationtypeType}")]
        [HttpGet]

        public async Task<IActionResult> getMRelationTypeByType(string MRelationtypeType)
        {
            try
            {
                var temp = await this.mRelationtypeService.GetEntityByType(MRelationtypeType);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MRelationtype doesn't exist with this type.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MRelationtype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMRelationTypes")]
        [HttpGet]
        public async Task<IActionResult> getAllMRelationTypes()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mRelationtypeService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MRelationtype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMRelationType")]
        [HttpPost]

        public async Task<IActionResult> UpdateMRelationType([FromBody] MRelationtypeUpdateModel model)
        {
            try
            {
                var temp = await this.mRelationtypeService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MRelationtype doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Type))
                    temp.Type = model.Type;
                if (!string.IsNullOrEmpty(model.Icon))
                    temp.Icon = model.Icon;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mRelationtypeService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MRelationtype updated successfully."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        #endregion

        #region MSchool

        [Route("AddMSchool")]
        [HttpPost]
        public async Task<IActionResult> AddMSchool([FromBody] MSchoolModel model)
        {
            try
            {
                var temp = await this.mSchoolService.GetEntityByName(model.Name);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MSchool already Exists."
                    });
                }
                var id = await this.mSchoolService.AddEntity(new MSchool
                {
                    Name = model.Name,
                    Description = model.Description,
                    Websitelink = model.Websitelink,
                    Emailid = model.Emailid,
                    Primaryphonenumber = model.Primaryphonenumber,
                    Secondaryphonenumber = model.Secondaryphonenumber,
                    Staffcount = model.Staffcount,
                    Logo = model.Logo,
                    Allowcategory = model.Allowcategory,
                    Issbsms = model.Issbsms,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMSchoolByID/{MSchoolID}")]
        [HttpGet]

        public async Task<IActionResult> getMSchoolByID(int MSchoolID)
        {
            try
            {
                var temp = await this.mSchoolService.GetEntityByID(MSchoolID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MSchool doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MSchool successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMSchoolByName/{MSchoolName}")]
        [HttpGet]

        public async Task<IActionResult> getMSchoolByName(string MSchoolName)
        {
            try
            {
                var temp = await this.mSchoolService.GetEntityByName(MSchoolName);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MSchool doesn't exist with this name.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    Message = "MSchool successfully fetched.",
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMSchoolByCodeSp/{code}")]
        [HttpGet]

        public async Task<IActionResult> getMSchoolByCodeSp(string code)
        {
            try
            {
                var temp = await this.mSchoolService.GetAllSchoolByCode(code);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "Code doesn't exist with this name.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    Message = "Code successfully fetched.",
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getAllMSchools")]
        [HttpGet]
        public async Task<IActionResult> getAllMSchools()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mSchoolService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MSchools successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMSchoolBySp")]
        [HttpGet]
        public IActionResult getAllMSchoolBySp()
        {
            try
            {
                return Ok(new
                {
                    Data = this.mSchoolService.GetAllEntitiesBySp(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MSchools successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMSchool")]
        [HttpPost]

        public async Task<IActionResult> UpdateMSchool([FromBody] MSchoolUpdateModel model)
        {
            try
            {
                var temp = await this.mSchoolService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MSchool doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Code))
                    temp.Code = model.Code;
                if (!string.IsNullOrEmpty(model.Description))
                    temp.Description = model.Description;
                if (!string.IsNullOrEmpty(model.Websitelink))
                    temp.Websitelink = model.Websitelink;
                if (!string.IsNullOrEmpty(model.Emailid))
                    temp.Emailid = model.Emailid;
                if (!string.IsNullOrEmpty(model.Logo))
                    temp.Logo = model.Logo;
                if (!string.IsNullOrEmpty(model.Primaryphonenumber))
                    temp.Primaryphonenumber = model.Primaryphonenumber;
                if (!string.IsNullOrEmpty(model.Secondaryphonenumber))
                    temp.Secondaryphonenumber = model.Secondaryphonenumber;
                if (model.Staffcount.HasValue)
                    temp.Staffcount = model.Staffcount;
                if (model.Allowcategory.HasValue)
                    temp.Allowcategory = model.Allowcategory;
                if (model.Issbsms.HasValue)
                    temp.Issbsms = model.Issbsms;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mSchoolService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MSchool updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        #endregion

        #region MStatus

        [Route("AddMStatus")]
        [HttpPost]
        public async Task<IActionResult> AddMStatus([FromBody] MStatusModel model)
        {
            try
            {
                var temp = await this.mStatusService.GetEntityByName(model.Name);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MStatus already Exists."
                    });
                }
                var id = await this.mStatusService.AddEntity(new MStatus
                {
                    Name = model.Name,
                    Description = model.Description,
                    Createdby = model.Createdby,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                    Isactive = model.Isactive
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMStatusByID/{MStatusID}")]
        [HttpGet]

        public async Task<IActionResult> getMStatusByID(int MStatusID)
        {
            try
            {
                var temp = await this.mStatusService.GetEntityByID(MStatusID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStatus doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStatus successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMStatusByName/{MStatusName}")]
        [HttpGet]

        public async Task<IActionResult> getMStatusByName(string MStatusName)
        {
            try
            {
                var temp = await this.mStatusService.GetEntityByName(MStatusName);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MStatus doesn't exist with this name.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    Message = "MStatus successfully fetched.",
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMStatus")]
        [HttpGet]
        public async Task<IActionResult> getAllMStatus()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mStatusService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStatus successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMStatus")]
        [HttpPost]

        public async Task<IActionResult> UpdateMStatus([FromBody] MStatusUpdateModel model)
        {
            try
            {
                var temp = await this.mStatusService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStatus doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Description))
                    temp.Description = model.Description;
                if (model.Statustypeid > 0)
                    temp.Statustypeid = model.Statustypeid;
                if (model.Createdby > 0)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mStatusService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStatus updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        #endregion

        #region MStatustype

        [Route("AddMStatustype")]
        [HttpPost]
        public async Task<IActionResult> AddMStatustype([FromBody] MStatustypeModel model)
        {
            try
            {
                var temp = await this.mStatustypeService.GetEntityByName(model.Name);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MStatustype already Exists."
                    });
                }
                var id = await this.mStatustypeService.AddEntity(new MStatustype
                {
                    Name = model.Name,
                    Description = model.Description,
                    Createdby = model.Createdby,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMStatustypeByID/{MStatustypeID}")]
        [HttpGet]

        public async Task<IActionResult> getMStatustypeByID(int MStatustypeID)
        {
            try
            {
                var temp = await this.mStatustypeService.GetEntityByID(MStatustypeID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStatustype doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStatustype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("getMStatustypeByName/{MStatustypeName}")]
        [HttpGet]

        public async Task<IActionResult> getMStatustypeByName(string MStatustypeName)
        {
            try
            {
                var temp = await this.mStatustypeService.GetEntityByName(MStatustypeName);
                if (temp.Count() == 0)
                {
                    return NotFound(new
                    {
                        Data = "MStatustype doesn't exist with this name.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    Message = "MStatustype successfully fetched.",
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMStatustype")]
        [HttpGet]
        public async Task<IActionResult> getAllMStatustype()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mStatustypeService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStatustype successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMStatustype")]
        [HttpPost]

        public async Task<IActionResult> UpdateMStatustype([FromBody] MStatustypeUpdateModel model)
        {
            try
            {
                var temp = await this.mStatustypeService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStatustype doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Description))
                    temp.Description = model.Description;
                if (model.Createdby > 0)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mStatustypeService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MStatustype updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        #endregion

        #region MChildSchoolMapping
        [Route("AddMChildSchoolMapping")]
        [HttpPost]
        public async Task<IActionResult> AddMChildSchoolMapping([FromBody] MChildschoolmappingModel model)
        {
            try
            {
                var id = await this.mChildschoolmappingService.AddEntity(new MChildschoolmapping
                {
                    Standardsectionmappingid = model.Standardsectionmappingid,
                    Childid = model.Childid,
                    Statusid = model.Statusid,
                    Createdby = model.Createdby,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Inserted successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMChildSchoolMappings")]
        [HttpGet]
        public async Task<IActionResult> getAllMChildSchoolMappings()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mChildschoolmappingService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }



        [Route("getMChildSchoolMappingByID/{Id}")]
        [HttpGet]
        public async Task<IActionResult> getMChildSchoolMappingByID(int Id)
        {
            try
            {
                var temp = await this.mChildschoolmappingService.GetEntityByID(Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "Child-School mapping doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Child-School mapping successfully fetched."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("UpdateMChildSchoolMapping")]
        [HttpPost]
        public async Task<IActionResult> UpdateMChildSchoolMapping(MChildschoolmappingUpdateModel model)
        {
            try
            {
                MChildschoolmapping temp = await this.mChildschoolmappingService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Msg = "Child-School mapping not found"
                    });
                }
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Standardsectionmappingid.HasValue)
                    temp.Standardsectionmappingid = model.Standardsectionmappingid;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;
                return Ok(new
                {
                    Data = await this.mChildschoolmappingService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Updated successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("DeleteMChildSchoolMapping/{Id}/{ModifiedBy}")]
        [HttpPost]
        public async Task<IActionResult> DeleteMChildSchoolMapping(int Id, int ModifiedBy)
        {
            try
            {
                if (DeleteStatusID > 0)
                {
                    MChildschoolmapping temp = await this.mChildschoolmappingService.GetEntityIDForUpdate(Id);
                    if (temp == null)
                    {
                        return BadRequest(new
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            Msg = "Child-School Mapping not found"
                        });
                    }
                    temp.Statusid = DeleteStatusID;
                    temp.Modifiedby = ModifiedBy;
                    temp.Modifieddate = DateTime.UtcNow;
                    return Ok(new
                    {
                        Data = await this.mChildschoolmappingService.UpdateEntity(temp),
                        StatusCode = HttpStatusCode.OK,
                        Msg = "Deleted successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Msg = "Deleted  unsuccessful"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }
        #endregion

        #region MParentChildMapping
        [Route("AddMParentChildMapping")]
        [HttpPost]
        public async Task<IActionResult> AddMParentChildMapping([FromBody] MParentchildmappingModel model)
        {
            try
            {
                var id = await this.mParentchildmappingService.AddEntity(new MParentchildmapping
                {
                    Relationtypeid = model.Relationtypeid,
                    Appuserid = model.Appuserid,
                    Childid = model.Childid,
                    Statusid = model.Statusid,
                    Createdby = model.Createdby,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Inserted successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("getAllMParentChildMappings")]
        [HttpGet]
        public async Task<IActionResult> getAllMParentChildMappings()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mParentchildmappingService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }



        [Route("getMParentChildMappingByID/{Id}")]
        [HttpGet]
        public async Task<IActionResult> getMParentChildMappingByID(int Id)
        {
            try
            {
                var temp = await this.mParentchildmappingService.GetEntityByID(Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "Parent-Child mapping doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Parent-Child mapping successfully fetched."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("UpdateMParentChildMapping")]
        [HttpPost]
        public async Task<IActionResult> UpdateMParentChildMapping(MParentchildmappingUpdateModel model)
        {
            try
            {
                MParentchildmapping temp = await this.mParentchildmappingService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Msg = "Parent-Child mapping not found"
                    });
                }
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Appuserid.HasValue)
                    temp.Appuserid = model.Appuserid;
                if (model.Relationtypeid.HasValue)
                    temp.Relationtypeid = model.Relationtypeid;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;
                return Ok(new
                {
                    Data = await this.mParentchildmappingService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Updated successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("DeleteMParentChildMapping/{Id}/{ModifiedBy}")]
        [HttpPost]
        public async Task<IActionResult> DeleteMParentChildMapping(int Id, int ModifiedBy)
        {
            try
            {
                if (DeleteStatusID > 0)
                {
                    MParentchildmapping temp = await this.mParentchildmappingService.GetEntityIDForUpdate(Id);
                    if (temp == null)
                    {
                        return BadRequest(new
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            Msg = "Parent-Child Mapping not found"
                        });
                    }
                    temp.Statusid = DeleteStatusID;
                    temp.Modifiedby = ModifiedBy;
                    temp.Modifieddate = DateTime.UtcNow;
                    return Ok(new
                    {
                        Data = await this.mParentchildmappingService.UpdateEntity(temp),
                        StatusCode = HttpStatusCode.OK,
                        Msg = "Deleted successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Msg = "Deletion  unsuccessful"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }
        #endregion

        #region MUsermodulemapping
        [Route("AddMUsermodulemapping")]
        [HttpPost]
        public async Task<IActionResult> AddMUsermodulemapping([FromBody] MUsermodulemappingModel model)
        {
            try
            {
                var id = await this.mUsermodulemappingService.AddEntity(new MUsermodulemapping
                {
                    Schooluserid = model.Schooluserid,
                    Moduleid = model.Moduleid,
                    Statusid = model.Statusid,
                    Createdby = model.Createdby,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Inserted successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("GetAllMUsermodulemappings")]
        [HttpGet]
        public async Task<IActionResult> GetAllMUsermodulemappings()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mUsermodulemappingService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }



        [Route("GetMUsermodulemappingByID/{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetMUsermodulemappingByID(int Id)
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mUsermodulemappingService.GetEntityByID(Id),
                    StatusCode = HttpStatusCode.OK,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("UpdateMUsermodulemapping")]
        [HttpPost]
        public async Task<IActionResult> UpdateMUsermodulemapping(MUsermodulemappingUpdateModel model)
        {
            try
            {
                MUsermodulemapping temp = await this.mUsermodulemappingService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Msg = "User-Module Mapping not found"
                    });
                }
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Moduleid.HasValue)
                    temp.Moduleid = model.Moduleid;
                if (model.Schooluserid.HasValue)
                    temp.Schooluserid = model.Schooluserid;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;
                return Ok(new
                {
                    Data = await this.mUsermodulemappingService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Updated successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("DeleteMUsermodulemapping/{Id}/{ModifiedBy}")]
        [HttpPost]
        public async Task<IActionResult> DeleteMUsermodulemapping(int Id, int ModifiedBy)
        {
            try
            {
                if (DeleteStatusID > 0)
                {
                    MUsermodulemapping temp = await this.mUsermodulemappingService.GetEntityIDForUpdate(Id);
                    if (temp == null)
                    {
                        return BadRequest(new
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            Msg = "User-Module Mapping not found"
                        });
                    }
                    temp.Statusid = DeleteStatusID;
                    temp.Modifiedby = ModifiedBy;
                    temp.Modifieddate = DateTime.UtcNow;
                    return Ok(new
                    {
                        Data = await this.mUsermodulemappingService.UpdateEntity(temp),
                        StatusCode = HttpStatusCode.OK,
                        Msg = "Deleted successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Msg = "Deletion  unsuccessful"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }
        #endregion

        #region Appuserdevice
        [Route("AddAppuserdevice")]
        [HttpPost]
        public async Task<IActionResult> AddAppuserdevice([FromBody] AppUserDevice model)
        {
            try
            {
                var id = await this.AppuserdeviceService.AddEntity(new Appuserdevice
                {
                    Groupid = model.Groupid,
                    Appuserid = model.Appuserid,
                    Deviceid = model.Deviceid,
                    Devicetype = model.Statusid,
                    Version = model.Version,
                    Createdby = model.Createdby,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Inserted successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("GetAllAppuserdevices")]
        [HttpGet]
        public async Task<IActionResult> GetAllAppuserdevices()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.AppuserdeviceService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }



        [Route("GetAppuserdeviceByID/{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetAppuserdeviceByID(int Id)
        {
            try
            {

                return Ok(new
                {
                    Data = await this.AppuserdeviceService.GetEntityByID(Id),
                    StatusCode = HttpStatusCode.OK,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("UpdateAppuserdevice")]
        [HttpPost]
        public async Task<IActionResult> UpdateAppuserdevice(AppUserDeviceUpdateModel model)
        {
            try
            {
                Appuserdevice temp = await this.AppuserdeviceService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Msg = "App-User Mapping not found"
                    });
                }
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Appuserid.HasValue)
                    temp.Appuserid = model.Appuserid;
                if (!string.IsNullOrEmpty(model.Deviceid))
                    temp.Deviceid = model.Deviceid;
                if (model.Groupid.HasValue)
                    temp.Groupid = model.Groupid;
                if (!string.IsNullOrEmpty(model.Version))
                    temp.Version = model.Version;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;
                return Ok(new
                {
                    Data = await this.AppuserdeviceService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Updated successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [Route("DeleteAppuserdevice/{Id}/{ModifiedBy}")]
        [HttpPost]
        public async Task<IActionResult> DeleteAppuserdevice(int Id, int ModifiedBy)
        {
            try
            {
                if (DeleteStatusID > 0)
                {
                    Appuserdevice temp = await this.AppuserdeviceService.GetEntityIDForUpdate(Id);
                    if (temp == null)
                    {
                        return BadRequest(new
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            Msg = "App-User Mapping not found"
                        });
                    }
                    temp.Statusid = DeleteStatusID;
                    temp.Modifiedby = ModifiedBy;
                    temp.Modifieddate = DateTime.UtcNow;
                    return Ok(new
                    {
                        Data = await this.AppuserdeviceService.UpdateEntity(temp),
                        StatusCode = HttpStatusCode.OK,
                        Msg = "Deleted successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Msg = "Deletion  unsuccessful"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }
        #endregion

        #region MFeature

        [Route("AddMFeature")]
        [HttpPost]
        public async Task<IActionResult> AddMFeature([FromBody] MFeatureUpdateModel model)
        {
            try
            {
                var temp = await this.mFeatureService.GetEntityByID(model.Id);
                if (temp != null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MFeature already Exists."
                    });
                }
                var id = await this.mFeatureService.AddEntity(new MFeature
                {
                    Schoolid = model.Schoolid,
                    Maxmsgcount = model.Maxmsgcount,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        [Route("getMFeatureByID/{MFeatureID}")]
        [HttpGet]
        public async Task<IActionResult> getMFeatureByID(int MFeatureID)
        {
            try
            {
                var temp = await this.mFeatureService.GetEntityByID(MFeatureID);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MFeature doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = temp,
                    StatusCode = HttpStatusCode.OK,
                    Message = "MFeature successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }

        [Route("getAllMFeatures")]
        [HttpGet]
        public async Task<IActionResult> getAllMFeatures()
        {
            try
            {

                return Ok(new
                {
                    Data = await this.mFeatureService.GetAllEntities(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MFeatures successfully fetched."
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }


        [Route("UpdateMFeature")]
        [HttpPost]

        public async Task<IActionResult> UpdateMFeature([FromBody] MFeatureUpdateModel model)
        {
            try
            {
                var temp = await this.mFeatureService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MFeature doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (model.Schoolid.HasValue)
                    temp.Schoolid = model.Schoolid;
                if (model.Maxmsgcount.HasValue)
                    temp.Maxmsgcount = model.Maxmsgcount;
                if (model.Statusid.HasValue)
                    temp.Statusid = model.Statusid;
                if (model.Createdby.HasValue)
                    temp.Createdby = model.Createdby;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mFeatureService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MFeature updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        #endregion


        #region Api for SendSms
        [HttpPost]
        [Route("api/SendSms")]
        public string SendSms(string toNumber, string message, string token)
        {

           
            //var userId = db.Tokens.FirstOrDefault(w => w.Id.ToString() == token && w.IsActive == true);
            //var childInfoModels = new List<ChildInfoModel>();
            //if (userId != null)
            //{
            //var appUserId = userId.AppUserId;

            //var schoolUser = db.AppUsers.FirstOrDefault(w => w.Id == appUserId);
            //if (schoolUser != null)
            //{
            //var mask = "";

            //switch (Convert.ToString(schoolUser.SchoolId).ToUpper())
            //{
            //    case "4F294817-6C46-4486-8761-73D34E056861":
            //        mask = "BISHOPS";
            //        break;
            //    case "B6657B66-80F9-4836-A846-82D34CB13EF4":
            //        mask = "BC STAFF";
            //        break;
            //    case "741BE775-5B0B-4B65-99FE-269FCF4C713F":
            //        mask = "STHOMASPREP";
            //        break;
            //    case "9682F56D-544F-428D-AA50-DC21F81E0CDD":
            //        mask = "STPS STAFF";
            //        break;
            //    case "0BC7C1F4-E1F4-4A4D-9B4D-96E063D3F00B":
            //        mask = "PRIME";
            //        break;
            //    case "130E910E-0844-457A-821C-F452BBD8F188":
            //        mask = "MARJORIEAPP";
            //        break;
            //    default:
            //        mask = "GETTALKTIVE";
            //        break;
            //}

            //Values Hardcoded
            var mask = "GETTALKTIVE";
            toNumber = "94777688250,8951745003,9036615609,9448635667,9480497627";
            message = "Multiple Number Testing";

            toNumber = SanitizeNumber(toNumber);
            //var user = db.AppUsers.FirstOrDefault(w => w.PhoneNumber == toNumber && w.IsActivated == true);
            if (toNumber != null)
            {
                MSMSService.SendSMS(toNumber.Split(','), message, mask);
                return "SMS sent";
            }
            else
                return "invalid request";
            //}
            //else
            //    return "invalid request";
            //}
            //else
            //    return "invalid request";
        }
        #endregion

        public static string AdminChannel = "87859B1B-053D-4EED-8Ay21-B69A6F3A02CA"; //System channel
        private string SanitizeNumber(string p)
        {
            return "+" + p.Replace("+", "").Replace(" ", "");
        }
        public enum ChannelType
        {
            AdminNotification, AdminToParent, AdminToSchool, AdminToAllSchools, AdminToCity, AdminToState, AdminToCountry, SchoolToParent, SchoolSoundingBoard, SchoolToAllInSchool, SchoolToStandard, SchoolToSection, ParentToParent, ParentToSection, ParentToSchoolCategory
        }
    }
}
