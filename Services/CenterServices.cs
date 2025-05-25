using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models.Enums;
using Repositories;
using System.Data.Entity.Core.Common;
using System.Linq.Expressions;

namespace Services
{
    public class CenterServices
    {
        private readonly CenterRepository centerRepository;
        public CommitData commitData;
        private readonly ProviderAssignmentRepository providerAssignmentRepository;
        private readonly ProviderRepository providerRepository;
        private readonly AccountServices accountServices;
        private readonly ProviderServices providerServices;

        public CenterServices(CenterRepository _centerRepository,
            CommitData _commitData,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ProviderRepository _providerRepository,
            AccountServices _accountServices, ProviderServices _providerServices)
        {
            centerRepository = _centerRepository;
            commitData = _commitData;
            providerAssignmentRepository = _providerAssignmentRepository;
            providerRepository = _providerRepository;
            accountServices = _accountServices;
            providerServices = _providerServices;
        }
        public List<Center> GetAll()
        {
            return centerRepository.GetAll().ToList();
        }
        public Center GetById(int id)
        {
            return centerRepository.GetById(c => c.CenterId == id);
        }
        public void Edit(Center entity)
        {
            centerRepository.Edit(entity);
            commitData.SaveChanges();
        }
        public bool Delete(int id)
        {
            try
            {
                var center = centerRepository.GetById(c => c.CenterId == id);
                if (center != null)
                {
                    centerRepository.Delete(center);
                    commitData.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Active(int id)
        {
            var center = centerRepository.GetById(c => c.CenterId == id);
            if (center != null)
            {
                centerRepository.Edit(center);
                commitData.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Inactive(int id)
        {
            var center = centerRepository.GetById(c => c.CenterId == id);
            if (center != null)
            {
                centerRepository.Edit(center);
                commitData.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public PaginationViewModel<CenterViewModel> Search(string searchText = "", int pageNumber = 1,
                                                            int pageSize = 2)
        {
            return centerRepository.Search(searchText, pageNumber, pageSize);
        }



        //public PaginationViewModel<ProviderViewModel> GetProvidersOfCenter(int CenterId)
        //{
        ////    var center = GetById(CenterId);
        ////    var result = center.ProviderAssignments.Where(c => c.CenterId == CenterId && c.IsDeleted == false).Select(res => res.Provider.toModelView()).ToList();
        //    var result = ProviderSearch(CenterId);
        //    return result;
        //}

        //public PaginationViewModel<ProviderViewModel> GetProvidersOfCenter(int centerId, int pageNumber, int pageSize, string sortBy, string specializationFilter)
        //{
        //    // Fetch provider assignments for the center
        //    var query = providerAssignmentRepository.GetList(pa => pa.CenterId == centerId && !pa.IsDeleted).ToList();

        //    // Map providers to assignments
        //    var providerAssignments = query.Select(pa =>
        //    {
        //        pa.Provider = providerRepository.GetProviderById(pa.ProviderId);
        //        return pa;
        //    }).Where(pa => pa.Provider != null).ToList();

        //    // Apply specialization filter
        //    if (!string.IsNullOrEmpty(specializationFilter))
        //    {
        //        providerAssignments = providerAssignments.Where(pa => pa.Provider.Specialization == specializationFilter).ToList();
        //    }

        //    // Sort the list based on sortBy
        //    var orderedAssignments = sortBy switch
        //    {
        //        "Specialization" => providerAssignments.OrderBy(pa => pa.Provider.Specialization).ToList(),
        //        "StartDate" => providerAssignmentRepository.FilterBy(
        //            filtereq: pa => providerAssignments.Contains(pa),
        //            Order_ColName: "StartDate",
        //            isAscending: true
        //        ).ToList(),
        //        _ => providerAssignmentRepository.FilterBy(
        //            filtereq: pa => providerAssignments.Contains(pa),
        //            Order_ColName: "StartDate",
        //            isAscending: true
        //        ).ToList()
        //    };

        //    // Pagination
        //    int total = orderedAssignments.Count;
        //    var paginatedAssignments = orderedAssignments
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    // Map to ProviderViewModel
        //    var result = paginatedAssignments.Select(pa => new ProviderViewModel
        //    {
        //        AssignmentId = pa.AssignmentId,
        //        ProviderId = pa.ProviderId,
        //        FirstName = pa.Provider.FirstName,
        //        LastName = pa.Provider.LastName,
        //        Specialization = pa.Provider.Specialization,
        //        Bio = pa.Provider.Bio,
        //        ExperienceYears = pa.Provider.ExperienceYears,
        //        LicenseNumber = pa.Provider.LicenseNumber,
        //        Gender = (GenderType)pa.Provider.Gender,
        //        Street = pa.Provider.Street,
        //        City = pa.Provider.City,
        //        Governorate = pa.Provider.Governorate,
        //        Country = pa.Provider.Country,
        //        BirthDate = pa.Provider.BirthDate,
        //        Image = pa.Provider.Image,
        //        Availability = pa.Provider.Availability,
        //        EstimatedDuration = pa.Provider.EstimatedDuration,
        //        AddDate = pa.StartDate // Adjust based on your ProviderAssignment model
        //    }).ToList();

        //    return new PaginationViewModel<ProviderViewModel>
        //    {
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        Total = total,
        //        Data = result
        //    };
        //}


        public PaginationViewModel<ProviderViewModel> GetProvidersOfCenter(int centerId, int pageNumber, int pageSize, string sortBy, string specializationFilter)
        {
            // Fetch provider assignments for the center with server-side filtering
            var builder = PredicateBuilder.New<ProviderAssignment>(true);
            builder = builder.And(pa => pa.CenterId == centerId && !pa.IsDeleted);

            // Pre-fetch providers to ensure they exist, avoiding null references later
            var assignments = providerAssignmentRepository.GetList(builder).ToList();
            var validAssignments = assignments
                .Select(pa =>
                {
                    pa.Provider = providerRepository.GetProviderById(pa.ProviderId);
                    return pa;
                })
                .Where(pa => pa.Provider != null) // Filter out assignments with null providers
                .ToList();

            // Apply specialization filter with case-insensitive comparison
            var filteredAssignments = validAssignments;
            if (!string.IsNullOrEmpty(specializationFilter) && specializationFilter != "All")
            {
                filteredAssignments = validAssignments
                    .Where(pa => pa.Provider.Specialization != null &&
                                 pa.Provider.Specialization.Equals(specializationFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Calculate total after filtering
            var total = filteredAssignments.Count;

            Console.WriteLine($"Applied specializationFilter: {specializationFilter}, Filtered count: {total}");

            // Apply sorting by StartDate
            var orderedAssignments = providerAssignmentRepository.FilterBy(
                filtereq: pa => filteredAssignments.Contains(pa),
                Order_ColName: "StartDate",
                isAscending: true
            ).ToList();

            Console.WriteLine($"Sorted by StartDate, Ordered count: {orderedAssignments.Count}");

            // Apply pagination
            var paginatedAssignments = orderedAssignments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map to ProviderViewModel
            var result = paginatedAssignments.Select(pa => new ProviderViewModel
            {
                AssignmentId = pa.AssignmentId,
                ProviderId = pa.ProviderId,
                FirstName = pa.Provider.FirstName,
                LastName = pa.Provider.LastName,
                Specialization = pa.Provider.Specialization,
                Bio = pa.Provider.Bio,
                ExperienceYears = pa.Provider.ExperienceYears,
                LicenseNumber = pa.Provider.LicenseNumber,
                Gender = (GenderType)pa.Provider.Gender,
                Street = pa.Provider.Street,
                City = pa.Provider.City,
                Governorate = pa.Provider.Governorate,
                Country = pa.Provider.Country,
                BirthDate = pa.Provider.BirthDate,
                Image = pa.Provider.Image,
                EstimatedDuration = pa.Provider.EstimatedDuration,
                AddDate = pa.StartDate,
                Status = GetProviderStatus(pa.ProviderId),

            }).ToList();


            return new PaginationViewModel<ProviderViewModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = total, // Now reflects the filtered count
                Data = result
            };
        }

        public async Task<string> GetProviderID(ProviderAssignmentDTO providerDto)
        {
            try
            {
                Console.WriteLine($"AddProviderAsync started at {DateTime.Now} (UTC: {DateTime.UtcNow}): centerId={providerDto.CenterId}, startDate={providerDto.WorkingDates?.FirstOrDefault()?.startDate}, endDate={providerDto.WorkingDates?.FirstOrDefault()?.endDate}, assignmentType={providerDto.AssignmentType}");


                var user = new RegisterationViewModel
                {
                    UserName = providerDto.UserName,
                    Email = providerDto.Email,
                    PhoneNumber = providerDto.PhoneNumber,
                    Password = providerDto.Password,
                    ConfirmPassword = providerDto.ConfirmPassword,
                    Role = providerDto.Role,
                    FirstName = providerDto.FirstName,
                    LastName = providerDto.LastName,
                    Gender = providerDto.Gender,
                    BirthDate = (DateOnly)providerDto.BirthDate,
                    Street = providerDto.Street,
                    City = providerDto.City,
                    Governorate = providerDto.Governorate,
                    Country = providerDto.Country,
                    Image = providerDto.Image,
                    Specialization = providerDto.Specialization,
                    LicenseNumber = providerDto.LicenseNumber
                };

                var userRes = await accountServices.CreateAccount(user);
                Console.WriteLine($"CreateAccount result at {DateTime.Now}: Succeeded={userRes.Succeeded}, Errors={string.Join(", ", userRes.Errors.Select(e => e.Description) ?? new[] { "No errors provided" })}");

                if (userRes.Succeeded)
                {
                    var providerId = await accountServices.getIdByUserName(providerDto.UserName);

                    return providerId;
                }

                return $"Not Valid";
            }

            catch
            {
                return $"Not Valid";
            }

        }

        public ProviderStatus GetProviderStatus(string providerId)
        {
            var assignments = providerAssignmentRepository.GetList(pa => pa.ProviderId == providerId && !pa.IsDeleted).ToList();

            foreach (var a in assignments)
            {
                foreach (var sh in a.Shifts)
                {
                    if (sh.LiveQueues.Count != 0)
                    {
                        return ProviderStatus.Online;
                    }
                }
            }

            return ProviderStatus.Offline;
        }

        public async Task<IdentityResult> AddProviderAsync(ProviderAssignmentDTO providerDto)
        {
            try
            {
                Console.WriteLine($"AddProviderAsync started at {DateTime.Now} (UTC: {DateTime.UtcNow}): centerId={providerDto.CenterId}, startDate={providerDto.WorkingDates?.FirstOrDefault()?.startDate}, endDate={providerDto.WorkingDates?.FirstOrDefault()?.endDate}, assignmentType={providerDto.AssignmentType}");


                var user = new RegisterationViewModel
                {
                    UserName = providerDto.UserName,
                    Email = providerDto.Email,
                    PhoneNumber = providerDto.PhoneNumber,
                    Password = providerDto.Password,
                    ConfirmPassword = providerDto.ConfirmPassword,
                    Role = providerDto.Role,
                    FirstName = providerDto.FirstName,
                    LastName = providerDto.LastName,
                    Gender = providerDto.Gender,
                    BirthDate = (DateOnly)providerDto.BirthDate,
                    Street = providerDto.Street,
                    City = providerDto.City,
                    Governorate = providerDto.Governorate,
                    Country = providerDto.Country,
                    Image = providerDto.Image,
                    Specialization = providerDto.Specialization,
                    LicenseNumber = providerDto.LicenseNumber
                };

                var userRes = await accountServices.CreateAccount(user);
                Console.WriteLine($"CreateAccount result at {DateTime.Now}: Succeeded={userRes.Succeeded}, Errors={string.Join(", ", userRes.Errors.Select(e => e.Description) ?? new[] { "No errors provided" })}");

                if (userRes.Succeeded)
                {
                    var providerId = await accountServices.getIdByUserName(providerDto.UserName);
                    Console.WriteLine($"ProviderId retrieved at {DateTime.Now}: {providerId ?? "null"}");



                    if (!string.IsNullOrEmpty(providerId))
                    {
                        foreach (var dates in providerDto.WorkingDates)
                        {

                            var providerAssignmentViewModel = new ProviderAssignmentViewModel
                            {
                                AssignmentType = providerDto.AssignmentType,
                                CenterId = providerDto.CenterId,
                                StartDate = dates.startDate,
                                EndDate = dates.endDate,
                                ProviderId = providerId,
                                Shifts = new List<ShiftViewModel>()
                            };

                            var assignmentResult = providerServices.AssignProviderToCenter(providerAssignmentViewModel);
                            Console.WriteLine($"AssignProviderToCenter result at {DateTime.Now}: {assignmentResult}");

                            if (assignmentResult == "Provider assigned successfully!")
                            {
                                try
                                {
                                    commitData.SaveChanges();
                                    Console.WriteLine($"SaveChanges succeeded at {DateTime.Now} for Provider {providerId} to Center {providerDto.CenterId}");
                                    // return IdentityResult.Success;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"SaveChanges failed at {DateTime.Now}: {ex.Message}, StackTrace: {ex.StackTrace}");
                                    return IdentityResult.Failed(new IdentityError { Description = $"Database save failed: {ex.Message}" });
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Assignment failed at {DateTime.Now}: {assignmentResult}");
                                return IdentityResult.Failed(new IdentityError { Description = assignmentResult });
                            }
                        }
                        return IdentityResult.Success;

                    }
                    else
                    {
                        Console.WriteLine($"Provider ID not found for username {providerDto.UserName} at {DateTime.Now}");
                        return IdentityResult.Failed(new IdentityError { Description = "Provider ID not found." });
                    }
                }
                else
                {
                    var errors = userRes.Errors.Any() ? userRes.Errors : new[] { new IdentityError { Description = "Failed to create user account: Unknown error." } };
                    Console.WriteLine($"CreateAccount failed at {DateTime.Now}, returning userRes: {string.Join(", ", errors.Select(e => e.Description))}");
                    return IdentityResult.Failed(errors.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddProviderAsync at {DateTime.Now}: {ex.Message}, StackTrace: {ex.StackTrace}");
                return IdentityResult.Failed(new IdentityError { Description = $"Error: {ex.Message}" });
            }

        }

       

        //error
        //public async Task<IdentityResult> AddProviderAsync(RegisterationViewModel user, int centerId, DateOnly _startdate, DateOnly _enddate, AssignmentType _assignmentType)
        //{
        //    try
        //    {
        //        var userRes = await accountServices.CreateAccount(user);

            //        if (userRes.Succeeded)
            //        {
            //            var providerId = await accountServices.getIdByUserName(user.UserName);

            //            if (!string.IsNullOrEmpty(providerId))
            //            {
            //                var providerAssignmentViewModel = new ProviderAssignmentViewModel
            //                {
            //                    AssignmentType = _assignmentType,
            //                    CenterId = centerId,
            //                    EndDate = _enddate,
            //                    StartDate = _startdate,
            //                    ProviderId = providerId
            //                };

            //                providerServices.AssignProviderToCenter(providerAssignmentViewModel);
            //                commitData.SaveChanges();
            //            }
            //        }

            //        return userRes;
            //    }
            //    catch (Exception ex)
            //    {
            //        // log exception or handle error
            //        return IdentityResult.Failed(new IdentityError { Description = $"Error: {ex.Message}" });
            //    }
            //}

            //public async Task<IdentityResult> AddProviderAsync(RegisterationViewModel user, int centerId, List<WorkingDates> workingDates, AssignmentType _assignmentType)
            //{
            //    try
            //    {
            //        Console.WriteLine($"AddProviderAsync started at {DateTime.Now} (UTC: {DateTime.UtcNow}): centerId={centerId}, startDate={_startdate}, endDate={_enddate}, assignmentType={_assignmentType}");
            //        var userRes = await accountServices.CreateAccount(user);
            //        Console.WriteLine($"CreateAccount result at {DateTime.Now}: Succeeded={userRes.Succeeded}, Errors={string.Join(", ", userRes.Errors.Select(e => e.Description) ?? new[] { "No errors provided" })}");

            //        if (userRes.Succeeded)
            //        {
            //            var providerId = await accountServices.getIdByUserName(user.UserName);
            //            Console.WriteLine($"ProviderId retrieved at {DateTime.Now}: {providerId ?? "null"}");

            //            if (!string.IsNullOrEmpty(providerId))
            //            {
            //                var providerAssignmentViewModel = new ProviderAssignmentViewModel
            //                {
            //                    AssignmentType = _assignmentType,
            //                    CenterId = centerId,
            //                    EndDate = _enddate,
            //                    StartDate = _startdate,
            //                    ProviderId = providerId,
            //                    Shifts = new List<ShiftViewModel>()
            //                };

            //                var assignmentResult = providerServices.AssignProviderToCenter(providerAssignmentViewModel);
            //                Console.WriteLine($"AssignProviderToCenter result at {DateTime.Now}: {assignmentResult}");

            //                if (assignmentResult == "Provider assigned successfully!")
            //                {
            //                    try
            //                    {
            //                        commitData.SaveChanges();
            //                        Console.WriteLine($"SaveChanges succeeded at {DateTime.Now} for Provider {providerId} to Center {centerId}");
            //                        return IdentityResult.Success;
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        Console.WriteLine($"SaveChanges failed at {DateTime.Now}: {ex.Message}, StackTrace: {ex.StackTrace}");
            //                        return IdentityResult.Failed(new IdentityError { Description = $"Database save failed: {ex.Message}" });
            //                    }
            //                }
            //                else
            //                {
            //                    Console.WriteLine($"Assignment failed at {DateTime.Now}: {assignmentResult}");
            //                    return IdentityResult.Failed(new IdentityError { Description = assignmentResult });
            //                }
            //            }
            //            else
            //            {
            //                Console.WriteLine($"Provider ID not found for username {user.UserName} at {DateTime.Now}");
            //                return IdentityResult.Failed(new IdentityError { Description = "Provider ID not found." });
            //            }
            //        }
            //        else
            //        {
            //            var errors = userRes.Errors.Any() ? userRes.Errors : new[] { new IdentityError { Description = "Failed to create user account: Unknown error." } };
            //            Console.WriteLine($"CreateAccount failed at {DateTime.Now}, returning userRes: {string.Join(", ", errors.Select(e => e.Description))}");
            //            return IdentityResult.Failed(errors.ToArray());
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Exception in AddProviderAsync at {DateTime.Now}: {ex.Message}, StackTrace: {ex.StackTrace}");
            //        return IdentityResult.Failed(new IdentityError { Description = $"Error: {ex.Message}" });
            //    }
            //}

            //delete provider from center
        public string DeleteProviderfromCenter(String ProviderId)
        {
            var assignment = providerAssignmentRepository.GetAssignmentByProviderId(ProviderId);
            assignment.IsDeleted = true;
            providerAssignmentRepository.Edit(assignment);
            commitData.SaveChanges();

            return "Provider Deleted Succesfully!";
        }


        //public PaginationViewModel<ProviderViewModel> ProviderSearch(string searchText = "", int pageNumber = 1,
        //int pageSize = 2)
        //{
        //    var builder = PredicateBuilder.New<Provider>(true);
        //    builder = builder.And(i => i.ProviderAssignments.Any(p => p.CenterId == centerId && p.IsDeleted == false));
        //    if (!searchText.IsNullOrEmpty())
        //    {
        //        builder = builder.And(i => (i.FirstName.ToLower().Contains(searchText.ToLower()) ||
        //        i.LastName.ToLower().Contains(searchText.ToLower()) ||
        //        i.City.ToLower().Contains(searchText.ToLower())
        //        && i.IsDeleted == false));
        //    }
        //    else
        //    {
        //        builder = builder.And(i => i.IsDeleted == false);
        //    }



        //    var count = providerRepository.GetList(builder).Count();
        //    var resultAfterPagination = providerRepository.Get(filter: builder, pageSize: pageSize, pageNumber: pageNumber).Select(p => p.toModelView()).ToList();
        //    return new PaginationViewModel<ProviderViewModel>
        //    {
        //        Data = resultAfterPagination,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        Total = count
        //    };
        //}


        public PaginationViewModel<ProviderViewModel> ProviderSearch(string searchText = "", int pageNumber = 1, int pageSize = 9, string specializationFilter = "")
        {
            var builder = PredicateBuilder.New<Provider>(true);

            if (!string.IsNullOrEmpty(searchText))
            {
                builder = builder.And(i => (i.FirstName.ToLower().Contains(searchText.ToLower()) ||
                                            i.LastName.ToLower().Contains(searchText.ToLower()) ||
                                            i.City.ToLower().Contains(searchText.ToLower())) &&
                                            i.IsDeleted == false);
            }
            else
            {
                builder = builder.And(i => i.IsDeleted == false);
            }

            if (!string.IsNullOrEmpty(specializationFilter))
            {
                builder = builder.And(i => i.Specialization != null && i.Specialization.ToLower() == specializationFilter.ToLower());
            }

            try
            {
                // Fetch all providers and ensure consistent ordering
                var allProviders = providerRepository.GetList(builder)
                    .OrderBy(p => p.ProviderId) // Ensure consistent ordering by a unique field
                    .ToList();
                var count = allProviders.Count();
                Console.WriteLine($"ProviderSearch - pageNumber: {pageNumber}, pageSize: {pageSize}, total count: {count}");

                // Ensure pageNumber is at least 1
                int adjustedPageNumber = Math.Max(1, pageNumber);
                int skip = (adjustedPageNumber - 1) * pageSize;
                var resultAfterPagination = allProviders
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(p => p.toModelView())
                    .ToList();

                // Log the IDs of the providers being returned for debugging
                var providerIds = resultAfterPagination.Select(p => p.ProviderId).ToList();
                Console.WriteLine($"ProviderSearch - adjusted pageNumber: {adjustedPageNumber}, skip: {skip}, returned data count: {resultAfterPagination.Count}, provider IDs: {string.Join(", ", providerIds)}");

                return new PaginationViewModel<ProviderViewModel>
                {
                    Data = resultAfterPagination,
                    PageNumber = adjustedPageNumber,
                    PageSize = pageSize,
                    Total = count
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ProviderSearch: {ex.Message} - StackTrace: {ex.StackTrace}");
                throw;
            }
        }

    }
}
