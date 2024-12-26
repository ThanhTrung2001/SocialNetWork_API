namespace EnVietSocialNetWorkAPI.Old.Repositories.Interface
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        public Task AddCompany(CreateCompany company);
    }
}
