using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Repositories;

namespace Services
{
    public class CenterServices
    {
        private readonly CommitData commitData;
        private readonly ProviderAssignmentRepository providerAssignmentRepository;
        private readonly AccountServices accountServices;
        private readonly ShiftRepository shiftRepository;
        private readonly CenterRepository centerRepository;
        private readonly AppointmentRepository appointmentRepository;
        private readonly ProviderRepository providerRepository;
        private readonly ClientRepository clientRepository;
        private readonly OperatorRepository operatorRepository;


        public CenterServices(CenterRepository _centerRepository,
            CommitData _commitData,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ProviderRepository _providerRepository,
            AccountServices _accountServices, ShiftRepository _shiftRepository, AppointmentRepository _appointmentRepository, ClientRepository _clientRepository, OperatorRepository _operatorRepository)
        {
            centerRepository = _centerRepository;
            commitData = _commitData;
            providerAssignmentRepository = _providerAssignmentRepository;
            providerRepository = _providerRepository;
            accountServices = _accountServices;
            shiftRepository = _shiftRepository;
            appointmentRepository = _appointmentRepository;
            clientRepository = _clientRepository;
            operatorRepository = _operatorRepository;
        }

        //public async Task<bool> CreateCenter(CenterDTO_ center)
        //{
        //    if (center is null)                          
        //        return false;

        //    var newCenter = new Center
        //    {
        //        CenterName = center.CenterName,
        //        ContactNumber = center.ContactNumber,
        //        Street = center.Street,
        //        City = center.City,
        //        Governorate = center.Governorate,
        //        Country = center.Country,
        //        Email = center.Email,
        //        WebsiteURL = center.WebsiteURL,
        //        Latitude = center.Latitude,
        //        Longitude = center.Longitude,
        //        MapURL = center.MapURL,
        //        CenterStatus = center.CenterStatus,
        //        IsDeleted = center.IsDeleted
        //    };

        //    centerRepository.Add(newCenter);
        //    commitData.SaveChanges();
        //    return true;
        //}




        public List<Center> GetAll()
        {
            return centerRepository.GetAll().ToList();
        }
        public Center GetCenterById(int id)
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


        public PaginationViewModel<ProviderViewModel> GetProvidersOfCenter(int centerId, int pageNumber, int pageSize, string sortBy, string specializationFilter)
        {

            var builder = PredicateBuilder.New<ProviderAssignment>(true);
            builder = builder.And(pa => pa.CenterId == centerId && !pa.IsDeleted);


            var assignments = providerAssignmentRepository.GetList(builder).ToList();
            var validAssignments = assignments
                .Select(pa =>
                {
                    pa.Provider = providerRepository.GetProviderById(pa.ProviderId);
                    return pa;
                })
                .Where(pa => pa.Provider != null)
                .ToList();

            var filteredAssignments = validAssignments;
            if (!string.IsNullOrEmpty(specializationFilter) && specializationFilter != "All")
            {
                filteredAssignments = validAssignments
                    .Where(pa => pa.Provider.Specialization != null &&
                                 pa.Provider.Specialization.Equals(specializationFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }


            var total = filteredAssignments.Count;

            Console.WriteLine($"Applied specializationFilter: {specializationFilter}, Filtered count: {total}");


            var orderedAssignments = providerAssignmentRepository.FilterBy(
                filtereq: pa => filteredAssignments.Contains(pa),
                Order_ColName: "StartDate",
                isAscending: true
            ).ToList();

            Console.WriteLine($"Sorted by StartDate, Ordered count: {orderedAssignments.Count}");


            var paginatedAssignments = orderedAssignments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            var result = paginatedAssignments.Select(pa => new ProviderViewModel
            {
                AssignmentId = pa.AssignmentId,
                ProviderId = pa.ProviderId,
                FirstName = pa.Provider.FirstName,
                LastName = pa.Provider.LastName,
                Specialization = pa.Provider.Specialization,
                Bio = pa.Provider.Bio,
                PhoneNumber = pa.Provider.User.PhoneNumber,
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
                Total = total,
                Data = result
            };
        }

        public List<ProviderDropDownDTO> GetProvidersOfCenterDropDown(int CenterId)
        {
            var builder = PredicateBuilder.New<ProviderAssignment>(true);
            builder = builder.And(pa => pa.CenterId == CenterId && !pa.IsDeleted);
            var assignments = providerAssignmentRepository.GetList(builder).ToList();
            var result = assignments
                .Select(pa => new ProviderDropDownDTO
                {
                    ProviderId = pa.ProviderId,
                    FullName = $"{pa.Provider.FirstName} {pa.Provider.LastName}",
                    Specialization = pa.Provider.Specialization
                })
                .ToList();
            return result;

        }

        public async Task<string> GetProviderID(ProviderAssignmentDTO providerDto)
        {
            try
            {


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

        public async Task<string> AddProviderAsync(RegisterationViewModel provider)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(provider.UserName))
                {
                    throw new ArgumentException("UserName is required.");
                }
                if (string.IsNullOrWhiteSpace(provider.Email))
                {
                    throw new ArgumentException("Email is required.");
                }
                if (string.IsNullOrWhiteSpace(provider.PhoneNumber))
                {
                    throw new ArgumentException("PhoneNumber is required.");
                }

                var userRes = await accountServices.CreateAccount(provider);

                if (userRes.Succeeded)
                {
                    var providerId = await accountServices.getIdByUserName(provider.UserName);

                    if (!string.IsNullOrEmpty(providerId))
                    {
                        try
                        {
                            commitData.SaveChanges();
                            return providerId;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Database save failed: {ex.Message}");
                        }
                    }
                    else
                    {
                        throw new Exception("Provider ID not found.");
                    }
                }
                else
                {
                    var errors = userRes.Errors.Any() ? userRes.Errors : new[] { new IdentityError { Description = "Failed to create user account: Unknown error." } };
                    throw new Exception(string.Join(", ", errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }




        //delete provider from center
        public string DeleteProviderfromCenter(string providerId, int centerId)
        {
            var assignments = providerAssignmentRepository.GetAll()
                .Where(pa => pa.ProviderId == providerId && pa.CenterId == centerId && !pa.IsDeleted)
                .ToList();

            if (!assignments.Any())
            {
                return "No assignments found for the provider and center.";
            }

            foreach (var assignment in assignments)
            {
                assignment.IsDeleted = true;

                var shifts = shiftRepository.GetAll()
                    .Where(s => s.ProviderAssignmentId == assignment.AssignmentId && !s.IsDeleted)
                    .ToList();

                foreach (var shift in shifts)
                {
                    shift.IsDeleted = true;
                    shift.ShiftType = ShiftType.Cancelled;

                    var appointments = appointmentRepository.GetAll()
                        .Where(a => a.ShiftId == shift.ShiftId)
                        .ToList();

                    foreach (var appointment in appointments)
                    {

                        appointment.AppointmentStatus = AppointmentStatus.Cancelled;
                        appointmentRepository.Edit(appointment);
                    }

                    shiftRepository.Edit(shift);
                }

                providerAssignmentRepository.Edit(assignment);
            }

            commitData.SaveChanges();

            return "Provider deleted from center successfully, including assignments, shifts, and appointments.";
        }


        public PaginationViewModel<ProviderViewModel> ProviderSearch(string searchText = "", int pageNumber = 1, int pageSize = 9, string specializationFilter = "", int centerId = 0)
        {
            var builder = PredicateBuilder.New<Provider>(true);


            builder = builder.And(i => i.IsDeleted == false);

            if (!string.IsNullOrEmpty(searchText))
            {
                builder = builder.And(i => (i.FirstName.ToLower().Contains(searchText.ToLower()) ||
                                            i.LastName.ToLower().Contains(searchText.ToLower()) ||
                                            i.City.ToLower().Contains(searchText.ToLower())));
            }

            if (!string.IsNullOrEmpty(specializationFilter))
            {
                builder = builder.And(i => i.Specialization != null && i.Specialization.ToLower() == specializationFilter.ToLower());
            }

            try
            {
                var assignedProviderIds = providerAssignmentRepository.GetList(pa => pa.CenterId == centerId && pa.IsDeleted == false)
                    .Select(pa => pa.ProviderId)
                    .ToList();

                if (assignedProviderIds.Any())
                {
                    builder = builder.And(p => !assignedProviderIds.Contains(p.ProviderId));
                }

                var allProviders = providerRepository.GetList(builder)
                    .OrderBy(p => p.ProviderId)
                    .ToList();

                var count = allProviders.Count();

                int adjustedPageNumber = Math.Max(1, pageNumber);
                int skip = (adjustedPageNumber - 1) * pageSize;
                var resultAfterPagination = allProviders
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(p => p.toModelView())
                    .ToList();

                var providerIds = resultAfterPagination.Select(p => p.ProviderId).ToList();

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
                throw;
            }
        }

        public CenterStatisticsDTO GetCenterStatistics(int centerId)

        {

            var center = centerRepository.GetById(c => c.CenterId == centerId);

            var providerCount = providerAssignmentRepository.GetList(pa => pa.CenterId == centerId && !pa.IsDeleted)
                .Select(pa => pa.ProviderId)
                .Distinct()
                .Count();

            var operatorCount = center?.Operators.Count(o => o.CenterId == centerId) ?? 0;

            var appointmentCount = appointmentRepository.GetAll()
                .Count(a => a.ProviderCenterService.CenterId == centerId);

            decimal totalRevenue = appointmentRepository.GetAll()
            .Where(a => a.ProviderCenterService.CenterId == centerId)
            .Sum(a => a.Fees + a.AdditionalFees);


            return new CenterStatisticsDTO
            {
                ProvidersCount = providerCount,
                OperatorsCount = operatorCount,
                AppointmentsCount = appointmentCount,
                TotalRevenue = totalRevenue
            };
        }
        public StatisticsViewModel GetStatisticsViewModel()
        {
            return new StatisticsViewModel
            {
                TotalCenters = centerRepository.GetAll().Count(c => !c.IsDeleted),
                TotalProviders = providerRepository.GetAll().Count(p => !p.IsDeleted),
                TotalOperators = operatorRepository.GetAll().Count(o => !o.IsDeleted),
                TotalClients = clientRepository.GetAll().Count(c => !c.IsDeleted),
                TotalShifts = shiftRepository.GetAll().Count(s => !s.IsDeleted),
                TotalAppointments = appointmentRepository.GetAll().Count(a => a.AppointmentStatus != AppointmentStatus.Cancelled),
            };
        }
    }
}
