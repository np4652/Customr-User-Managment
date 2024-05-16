using Microsoft.AspNetCore.Http;
using System.Data;
using System.Reflection;
using UserManagement.Domain.Base;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Services
{
    public class AuditTrail<T> : IAuditTrail<T> where T : class
    {
        private readonly IDbContext _context;
        private readonly string _applicationName = "UserManagement";
        private readonly string _createdBy = "";
        public AuditTrail(IDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _createdBy = httpContextAccessor.HttpContext.User.FindFirst("userName")?.Value ?? string.Empty;
        }
        public async Task DeleteAsync(T oldEntity)
        {
            if (oldEntity == null)
            {
                return;
            }


            // Get the properties from type
            Type type = oldEntity.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            var transaction = _context.Connection.BeginTransaction();
            try
            {
                string sqlQuery = @"INSERT INTO [AuditEntry] (EntitySetName, EntityTypeName, State, StateName, CreatedBy, CreatedDate, Discriminator, AppplicationName) 
                                    OUTPUT inserted.AuditEntryID
                                    VALUES (@EntitySetName, @EntityTypeName, @State,@StateName, @CreatedBy, GETDATE(), @Discriminator, @AppplicationName);
                                    SELECT SCOPE_IDENTITY();";
                int auditEntryId = await _context.ExecuteAsync(sqlQuery, new
                {
                    EntitySetName = type.Name,
                    EntityTypeName = type.Name,
                    State = 2,
                    StateName = "DELETE",
                    CreatedBy = _createdBy,
                    Discriminator = "CustomAuditEntry",
                    AppplicationName = _applicationName,
                }, commandType: CommandType.Text, transaction: transaction);


                // Insert into AuditEntryProperty
                // Values of each property

                sqlQuery = @"INSERT INTO [AuditEntryProperty] (AuditEntryID, RelationName, PropertyName, OldValue, NewValue, Discriminator, AppplicationName) 
                                    VALUES (@AuditEntryID, @RelationName, @PropertyName, @OldValue, @NewValue, @DiscriminatorProperty, @AppplicationNameProperty);
                                    SELECT SCOPE_IDENTITY();";

                List<AuditEntryProperty> properties = new List<AuditEntryProperty>();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    properties.Add(new AuditEntryProperty
                    {
                        AuditEntryID = auditEntryId,
                        RelationName = string.Empty,
                        PropertyName = propertyInfo.Name,
                        OldValue = Convert.ToString(propertyInfo.GetValue(oldEntity)),
                        NewValue = string.Empty,
                        DiscriminatorProperty = "CustomAuditEntryProperty",
                        AppplicationNameProperty = _applicationName,
                    });
                }
                auditEntryId = await _context.ExecuteAsync(sqlQuery, properties, commandType: CommandType.Text, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public async Task InsertAsync(T entity)
        {
            if (entity == null)
            {
                return;
            }
            Type type = typeof(T);
            try
            {
                string sqlQuery = @"INSERT INTO [AuditEntry] (EntitySetName, EntityTypeName, State, StateName, CreatedBy, CreatedDate, Discriminator, AppplicationName) 
                                    OUTPUT inserted.AuditEntryID
                                    VALUES (@EntitySetName, @EntityTypeName, @State,@StateName, @CreatedBy, GETDATE(), @Discriminator, @AppplicationName);
                                    SELECT SCOPE_IDENTITY();";
                int auditEntryId = await _context.ExecuteAsync(sqlQuery, new
                {
                    EntitySetName = type.Name,
                    EntityTypeName = type.Name,
                    State = 0,
                    StateName = "INSERT",
                    CreatedBy = _createdBy,
                    Discriminator = "CustomAuditEntry",
                    AppplicationName = _applicationName,
                }, commandType: CommandType.Text);
            }
            catch{}
            finally{}
        }

        public async Task UpdateAsync(T oldEntity, T newEntity)
        {
            if (oldEntity is null || newEntity is null)
            {
                return;
            }

            // Get the properties from type
            Type type = oldEntity.GetType();// typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();
            Type newType = newEntity.GetType();
            PropertyInfo[] newPropertyInfos = newType.GetProperties();
            var transaction = _context.Connection.BeginTransaction();
            try
            {
                List<AuditEntryProperty> properties = new List<AuditEntryProperty>();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    string newValue = Convert.ToString(newPropertyInfos.Where(x => x.Name == propertyInfo.Name).FirstOrDefault()?.GetValue(newEntity));
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        string oldValue = Convert.ToString(propertyInfo.GetValue(oldEntity));
                        if (oldValue != newValue)
                        {
                            properties.Add(new AuditEntryProperty
                            {
                                AuditEntryID = 0,
                                RelationName = string.Empty,
                                PropertyName = propertyInfo.Name,
                                OldValue = oldValue,
                                NewValue = newValue,
                                DiscriminatorProperty = "CustomAuditEntryProperty",
                                AppplicationNameProperty = _applicationName,
                            });
                        }
                    }
                }
                if (properties.Count > 0)
                {
                    // Insert into AuditEntry
                    string sqlQuery = @"INSERT INTO [AuditEntry] (EntitySetName, EntityTypeName, State, StateName, CreatedBy, CreatedDate, Discriminator, AppplicationName) 
                                    OUTPUT inserted.AuditEntryID
                                    VALUES (@EntitySetName, @EntityTypeName, @State,@StateName, @CreatedBy, GETDATE(), @Discriminator, @AppplicationName);
                                    SELECT SCOPE_IDENTITY();";
                    int auditEntryId = await _context.ExecuteAsync(sqlQuery, new
                    {
                        EntitySetName = type.Name,
                        EntityTypeName = type.Name,
                        State = 1,
                        StateName = "Modified",
                        CreatedBy = _createdBy,
                        Discriminator = "CustomAuditEntry",
                        AppplicationName = _applicationName,
                    }, commandType: CommandType.Text, transaction: transaction);

                    sqlQuery = @"INSERT INTO [AuditEntryProperty] (AuditEntryID, RelationName, PropertyName, OldValue, NewValue, Discriminator, AppplicationName) 
                                    VALUES (@AuditEntryID, @RelationName, @PropertyName, @OldValue, @NewValue, @DiscriminatorProperty, @AppplicationNameProperty);
                                    SELECT SCOPE_IDENTITY();";

                    // Insert into AuditEntryProperty table
                    var _properties = properties.Select(x => new
                    {
                        AuditEntryID = auditEntryId,
                        x.RelationName,
                        x.PropertyName,
                        x.OldValue,
                        x.NewValue,
                        x.AppplicationNameProperty,
                        x.DiscriminatorProperty
                    });
                    auditEntryId = await _context.ExecuteAsync(sqlQuery, _properties, commandType: CommandType.Text, transaction: transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }


    public class AuditEntryProperty
    {
        public int AuditEntryID { get; set; }
        public string RelationName { get; set; }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string DiscriminatorProperty { get; set; }
        public string AppplicationNameProperty { get; set; }
    }
}
