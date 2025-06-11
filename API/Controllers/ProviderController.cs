using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using System.Data.Entity.Core.Common;

namespace API.Controllers
{
    // [Authorize(Roles = "Provider")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderController : ControllerBase
    {
        public CenterServices centerServices;
        public ProviderServices providerServices;
        public OperatorServices operatorServices;
        public ShiftServices shiftServices;
        public LiveQueueServices liveQueueServices;
        LiveQueueRepository liveQueueRepository;
        public ProviderController(ProviderServices _providerServices, ShiftServices _shiftServices, LiveQueueServices _liveQueueServices, CenterServices _centerServices, OperatorServices _operatorServices, LiveQueueRepository _liveQueueRepository)
        {
            centerServices = _centerServices;
            providerServices = _providerServices;
            shiftServices = _shiftServices;
            liveQueueServices = _liveQueueServices;
            operatorServices = _operatorServices;
            liveQueueRepository = _liveQueueRepository;

        }

        [HttpGet("GetProviderById/{providerId}")]
        public async Task<IActionResult> GetProviderById(string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<ProviderViewModel> { Message = "Provider ID is required", Status = 400 });
            }

            try
            {
                var provider = await providerServices.GetProviderDetailsById(providerId);
                if (provider == null)
                {
                    return NotFound(new ApiResponse<ProviderViewModel> { Message = "Provider not found", Status = 404 });
                }

                return Ok(new ApiResponse<ProviderViewModel>
                {
                    Message = "Provider details retrieved successfully",
                    Status = 200,
                    Data = provider
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProviderById: {ex.Message} - StackTrace: {ex.StackTrace}");
                return StatusCode(500, new ApiResponse<ProviderViewModel> { Message = "Internal server error", Status = 500 });
            }
        }

        [HttpGet("{providerId}/assignments")]
        public IActionResult GetProviderAssignments(string providerId, int centerId)
        {
            try
            {
                var assignments = providerServices.GetProviderAssignments(providerId, centerId);

                if (!assignments.Any())
                {
                    return NotFound(new ApiResponse<object[]>
                    {
                        Message = "No assignments found for the given criteria.",
                        Status = 404
                    });
                }

                Console.WriteLine($"Assignments retrieved: {System.Text.Json.JsonSerializer.Serialize(assignments)}");
                return Ok(new ApiResponse<object[]>
                {
                    Message = "Assignments retrieved successfully",
                    Status = 200,
                    Data = assignments.ToArray()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProviderAssignments: {ex.Message} - StackTrace: {ex.StackTrace}");
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = "Internal server error",
                    Status = 500
                });
            }
        }

        [HttpGet("ScheduleDetails")]
        public IActionResult ScheduleDetails([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider ID is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider not found", Status = 404 });
            }
            List<GetProviderScheduleDetailsDTO> scheduleDetails = providerServices.GetScheduleDetails(provider);
            if (scheduleDetails == null || !scheduleDetails.Any())
            {
                return NotFound(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider schedule details not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetProviderScheduleDetailsDTO>>
            {
                Message = "Get Provider schedule details Successfully",
                Status = 200,
                Data = scheduleDetails
            });
        }
        [HttpGet("ShiftDetails")]
        public IActionResult ShiftDetails([FromQuery] int shiftId)
        {
            if (shiftId <= 0)
            {
                return BadRequest(new ApiResponse<GetShiftDetailsDTO>
                {
                    Message = "Invalid shift id",
                    Status = 400
                });
            }
            GetShiftDetailsDTO shiftDetails = providerServices.GetShiftDetails(shiftId);
            if (shiftDetails == null)
            {
                return NotFound(new ApiResponse<GetShiftDetailsDTO> { Message = "Shift details not found", Status = 404 });
            }
            return Ok(new ApiResponse<GetShiftDetailsDTO>
            {
                Message = "Get shift details Successfully",
                Status = 200,
                Data = shiftDetails
            });
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateDoctorProfile([FromBody] UpdateProviderProfileDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "Invalid data", Status = 400 });

            var result = await providerServices.UpdateDoctorProfile(model);

            return Ok(new ApiResponse<string>
            {
                Message = result,
                Status = 200,
                Data = result
            });
        }
        [HttpPut("update-professional-info")]
        public IActionResult UpdateProfessionalInfo([FromBody] UpdateProviderProfessionalInfoDTO model)
        {
            var result = providerServices.UpdateProfessionalInfo(model);

            return Ok(new ApiResponse<string>
            {
                Message = result,
                Status = 200,
                Data = result
            });
        }

        //[HttpGet("Queue-Entries")]
        //public IActionResult QueueEntries(string providerId)
        //{
        //    if (string.IsNullOrEmpty(providerId))
        //    {
        //        return BadRequest(new ApiResponse<GetQueueEntriesDTO> { Message = "Provider Id is required", Status = 400 });
        //    }
        //    Provider provider = providerServices.GetProviderById(providerId);
        //    if (provider == null)
        //    {
        //        return NotFound(new ApiResponse<List<GetQueueEntriesDTO>> { Message = "Provider not found", Status = 404 });
        //    }
        //    List<GetQueueEntriesDTO> queueEntries = liveQueueServices.GetQueueEntries(provider);
        //    if (queueEntries == null || !queueEntries.Any())
        //    {
        //        return NotFound(new ApiResponse<List<GetQueueEntriesDTO>> { Message = "Queue entries not found", Status = 404 });
        //    }
        //    return Ok(new ApiResponse<List<GetQueueEntriesDTO>>
        //    {
        //        Message = "Get queue entries successfully",
        //        Status = 200,
        //        Data = queueEntries
        //    });

        //}

        // [Authorize(Roles = "Admin, Operator")]
        [HttpGet]
        [Route("GetProviderLiveQueues")]
        public IActionResult GetProviderLiveQueues(string providerId, int centerId, int pageNumber = 1, int pageSize = 16)
        {
            try
            {
                // Validate providerId
                if (string.IsNullOrEmpty(providerId))
                {
                    return BadRequest(new ApiResponse<ProviderLiveQueueViewModel>
                    {
                        Message = "Provider Id is required",
                        Status = 400
                    });
                }

                // Check if provider exists
                var provider = providerServices.GetProviderById(providerId);
                if (provider == null)
                {
                    return NotFound(new ApiResponse<ProviderLiveQueueViewModel>
                    {
                        Message = "Provider not found",
                        Status = 404
                    });
                }

                // Fetch paginated live queues for the provider and center
                var liveQueues = liveQueueServices.GetLiveQueuesForProvider(providerId, centerId, pageNumber, pageSize);

                // Check if any live queues were found
                if (liveQueues.Data == null || !liveQueues.Data.Any())
                {
                    return NotFound(new ApiResponse<PaginationViewModel<ProviderLiveQueueViewModel>>
                    {
                        Message = "No live queues found for the specified provider and center",
                        Status = 404,
                        Data = new PaginationViewModel<ProviderLiveQueueViewModel>
                        {
                            Data = new List<ProviderLiveQueueViewModel>(),
                            PageNumber = pageNumber,
                            PageSize = pageSize,
                            Total = 0
                        }
                    });
                }

                return Ok(new ApiResponse<PaginationViewModel<ProviderLiveQueueViewModel>>
                {
                    Data = liveQueues,
                    Message = "Live queues retrieved successfully",
                    Status = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}",
                    Status = 500
                });
            }
            }

        // [Authorize(Roles = "Admin, Operator")]
        [HttpPost]
        [Route("UpdateLiveQueueStatus")]
        public IActionResult UpdateLiveQueueStatus([FromBody] UpdateQueueStatusViewModel model)
        {
            try
            {
                var result = operatorServices.UpdateQueueStatus(model);
                if (result.StartsWith("Queue status updated successfully"))
                {
                    return Ok(new ApiResponse<string>
                    {
                        Data = result,
                        Message = "Success",
                        Status = 200
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Data = result,
                        Message = "Failed",
                        Status = 400
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}",
                    Status = 500
                });
            }
        }
    }
    }
