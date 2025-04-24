namespace MyFitnessApp.Services.Data.Tests
{
    using System;
    using System.Threading.Tasks;

    using Moq;
    using MyFitnessApp.Data;
    using MyFitnessApp.Data.Common.Repositories;
    using MyFitnessApp.Data.Models;
    using MyFitnessApp.Data.Repositories;
    using MyFitnessApp.Services.Data.Profile;
    using MyFitnessApp.Web.ViewModels.Profiles;

    using Xunit;

    public class CustomProfileServiceTests : BaseServiceTests
    {
        private readonly Mock<IDeletableEntityRepository<Profile>> profilesRepository;
        private readonly Mock<IDeletableEntityRepository<ApplicationUser>> usersRepository;
        private readonly Mock<Data.Food.IFoodsService> foodsService;

        public CustomProfileServiceTests()
        {
            this.profilesRepository = new Mock<IDeletableEntityRepository<Profile>>();
            this.usersRepository = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            this.foodsService = new Mock<Data.Food.IFoodsService>();
        }

        [Fact]
        public void DoesUserHaveProfile_ShouldReturnFalse_WhenNoProfileExists()
        {
            ApplicationDbContext db = GetDb();

            var profilesRepository = new EfDeletableEntityRepository<Profile>(db);
            var service = new ProfilesService(profilesRepository, this.usersRepository.Object, this.foodsService.Object);

            var user = new ApplicationUser
            {
                Id = "noProfUser",
                UserName = "noprof",
                Email = "noprof@example.com",
            };

            db.Users.Add(user);
            db.SaveChanges();

            var result = service.DoesUserHaveProfile(user.Id);

            Assert.False(result);
        }

        [Fact]
        public void GetUserIdByUserName_ShouldReturnNull_WhenUserDoesNotExist()
        {
            ApplicationDbContext db = GetDb();

            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(db);
            var service = new ProfilesService(this.profilesRepository.Object, usersRepository, this.foodsService.Object);

            var result = service.GetUserIdByUserName("ghostuser");

            Assert.Null(result);
        }

        [Fact]
        public void GetProfileDataForUpdate_ShouldReturnNull_WhenProfileIsMissing()
        {
            ApplicationDbContext db = GetDb();

            var profilesRepository = new EfDeletableEntityRepository<Profile>(db);
            var service = new ProfilesService(profilesRepository, this.usersRepository.Object, this.foodsService.Object);

            var user = new ApplicationUser
            {
                Id = "u101",
                UserName = "noprof",
                Email = "noprof@example.com",
            };

            db.Users.Add(user);
            db.SaveChanges();

            var result = service.GetProfileDataForUpdate(user.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task EditProfileAsync_ShouldThrow_WhenUserDoesNotExist()
        {
            ApplicationDbContext db = GetDb();

            var profilesRepository = new EfDeletableEntityRepository<Profile>(db);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(db);

            var service = new ProfilesService(profilesRepository, usersRepository, this.foodsService.Object);

            var inputModel = new EditProfileInputModel
            {
                FirstName = "Ghost",
                LastName = "User",
                Email = "ghost@nowhere.com",
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.EditProfileAsync(inputModel, "ghost-id"));
        }

        [Fact]
        public async Task EditProfileAsync_ShouldUpdateUserDetails()
        {
            ApplicationDbContext db = GetDb();

            var profilesRepository = new EfDeletableEntityRepository<Profile>(db);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(db);

            var service = new ProfilesService(profilesRepository, usersRepository, this.foodsService.Object);

            var user = new ApplicationUser
            {
                Id = "user123",
                UserName = "user",
                FirstName = "OldFirst",
                LastName = "OldLast",
                Email = "old@example.com",
            };

            var profile = new Profile
            {
                AddedByUser = user,
                AboutMe = "Old bio",
            };

            await db.Users.AddAsync(user);
            await db.Profiles.AddAsync(profile);
            await db.SaveChangesAsync();

            var inputModel = new EditProfileInputModel
            {
                FirstName = "NewFirst",
                LastName = "NewLast",
                Email = "new@example.com",
                AboutMe = "Updated bio",
            };

            await service.EditProfileAsync(inputModel, user.Id);

            Assert.Equal("NewFirst", user.FirstName);
            Assert.Equal("NewLast", user.LastName);
            Assert.Equal("new@example.com", user.Email);
        }
    }
}
