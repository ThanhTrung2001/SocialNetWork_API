using Dapper;
using EV.DataAccess.DataConnection;
using EV.DataAccess.Helper;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;

namespace EV.DataAccess.Repositories
{
    public class OrganizationRepository
    {
        private readonly DatabaseContext _context;

        public OrganizationRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<OrganizeNodeQuery>>> Get()
        {
            var query = "SELECT * FROM Organizations;";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    // Fetch all nodes from the database
                    var result = await connection.QueryAsync<OrganizeNodeQuery>(query);
                    List<OrganizeNodeQuery> hierarchy = TreeHierachyHelper.BuildHierarchy(result.ToList());
                    return ResponseModel<IEnumerable<OrganizeNodeQuery>>.Success(hierarchy);
                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<OrganizeNodeQuery>>.Failure(ex.Message)!;
                }

            }
        }

        public async Task<ResponseModel<OrganizeNodeQuery>> GetById(Guid id)
        {
            var query = "SELECT * FROM Organizations WHERE Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    // Fetch all nodes from the database
                    var result = await connection.QueryAsync<OrganizeNodeQuery>(query, parameter);
                    OrganizeNodeQuery hierarchy = TreeHierachyHelper.BuildHierarchyChild(result.ToList(), id);
                    return ResponseModel<OrganizeNodeQuery>.Success(hierarchy);
                }
                catch (Exception ex)
                {
                    return ResponseModel<OrganizeNodeQuery>.Failure(ex.Message)!;
                }

            }
        }

        public async Task<ResponseModel<Guid>> Create(CreateOrganizeNodeCommand command)
        {
            var query = @"INSERT INTO Organizations (Id, Name, Description, Department, Email, Phone_Number, Address, City, Country, Level, Parent_Id, Employee_Count)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), @Name, @Description,  @Department, @Email, @Phone_Number, @Address, @City, @Country, @Level, @Parent_Id, 0)";
            var parameters = new DynamicParameters();
            parameters.Add("Name", command.Name);
            parameters.Add("Description", command.Description);
            parameters.Add("Department", command.Department);
            parameters.Add("Email", command.Email);
            parameters.Add("Phone_Number", command.Phone_Number);
            parameters.Add("Address", command.Address);
            parameters.Add("City", command.City);
            parameters.Add("Country", command.Country);
            parameters.Add("Level", command.Level);
            parameters.Add("Parent_Id", command.Parent_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<Guid>(query, parameters);
                    return ResponseModel<Guid>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<Guid>.Failure(ex.Message);
                }
            }
        }

        public async Task<ResponseModel<string>> Edit(Guid id, CreateOrganizeNodeCommand command)
        {
            var query = @"UPDATE Organizations 
                          SET 
                          Name = @Name, Department = @Department, Email = @Email, Phone_Number = @Phone_Number, Address = @Address, City = @City, Country = @Country ,Level = @Level, Parent_Id = @Parent_Id 
                          WHERE ID = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Name", command.Name);
            parameters.Add("Description", command.Description);
            parameters.Add("Department", command.Department);
            parameters.Add("Email", command.Email);
            parameters.Add("Phone_Number", command.Phone_Number);
            parameters.Add("Address", command.Address);
            parameters.Add("City", command.City);
            parameters.Add("Country", command.Country);
            parameters.Add("Level", command.Level);
            parameters.Add("Parent_Id", command.Parent_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Delete(Guid id)
        {
            var query = "UPDATE Organizations SET Is_Deleted = 1 WHERE Id = @Id";
            //Delete User_Organization

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        #region [User In Organization]

        public async Task<ResponseModel<IEnumerable<OrganizationUserQuery>>> GetUsersByOrganizationId(Guid id)
        {
            var query = @"SELECT 
                            o.Department,
                            uo.Organization_Role,
                            uo.User_Id,
                            ud.FirstName,
                            ud.LastName,
                            ud.Avatar
                          FROM Organizations o
                          INNER JOIN 
                            User_Organization uo ON o.Id = uo.Node_Id
                          LEFT JOIN
                            User_Details ud ON uo.User_Id = ud.User_Id
                          WHERE
                            uo.Is_Deleted = 0 AND o.Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<OrganizationUserQuery>(query, parameters);
                    return ResponseModel<IEnumerable<OrganizationUserQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<OrganizationUserQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> CreateUser(Guid id, CreateOrganizationUserCommand command)
        {
            //Check if existed
            var existQuery = @"SELECT COUNT(1) FROM User_Organization
                   WHERE User_Id = @User_Id AND Node_Id = @Organization_Id;";

            var query = @"INSERT INTO User_Organization (User_Id, Node_Id, Created_At, Updated_At, Is_Deleted, Organization_Role)
                          VALUES 
                          (@User_Id, @Organization_Id, GETDATE(), GETDATE(), 0, @Organization_Role)";
            var updateQuery = @"UPDATE Organizations SET Employee_Count = Employee_Count + 1 WHERE ID = @Organization_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Organization_Id", id);
            parameters.Add("Organization_Role", command.Organization_Role);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    bool existed = await connection.ExecuteScalarAsync<bool>(existQuery, parameters);
                    if (existed)
                    {
                        return ResponseModel<string>.Failure("Existed Connection between User and Organization.")!;
                    }
                    else
                    {
                        var rowEffect = await connection.ExecuteAsync(query, parameters);
                        var rowOrganizationEffect = await connection.ExecuteAsync(updateQuery, parameters);
                        return (rowEffect > 0 && rowOrganizationEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                    }

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }

            }
        }

        public async Task<ResponseModel<string>> EditUser(Guid id, EditOrganizationUserCommand command)
        {
            var query = "UPDATE User_Organization SET Organization_Role = @Organization_Role WHERE User_Id = @User_Id AND Node_Id = @Organization_Id;";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Organization_Id", id);
            parameters.Add("Organization_Role", command.Organization_Role);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> DeleteUser(Guid id, DeleteOrganizationUserCommand command)
        {
            var query = "UPDATE User_Organization SET Is_Deleted = 1 WHERE User_Id = @User_Id AND Node_Id = @Organization_Id";
            var updateQuery = @"UPDATE Organizations SET Employee_Count = Employee_Count - 1 WHERE ID = @Organization_Id";

            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Organization_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    var rowOrganizationEffect = await connection.ExecuteAsync(updateQuery, parameters);
                    return (rowEffect > 0 && rowOrganizationEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        #endregion
    }
}
