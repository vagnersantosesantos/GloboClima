using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using GloboClima.Core.Common;
using GloboClima.Core.Interfaces.Repositories;
using GloboClima.Core.Models;

namespace GloboClima.Infrastructure.Repositories
{
    public class DynamoDBUserRepository : IUserRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName = "GloboClima-Users";

        public DynamoDBUserRepository(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<ServiceResult<User>> GetByIdAsync(string id)
        {
            try
            {
                var request = new GetItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["Id"] = new AttributeValue { S = id }
                    }
                };

                var response = await _dynamoDb.GetItemAsync(request);

                if (response.Item.Count == 0)
                    return ServiceResult<User>.Failure("Usuário não encontrado");

                var user = MapDynamoDBItemToUser(response.Item);
                return ServiceResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failure($"Erro ao buscar usuário: {ex.Message}");
            }
        }

        public async Task<ServiceResult<User>> GetByEmailAsync(string email)
        {
            try
            {
                var request = new ScanRequest
                {
                    TableName = _tableName,
                    FilterExpression = "Email = :email",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [":email"] = new AttributeValue { S = email }
                    }
                };

                var response = await _dynamoDb.ScanAsync(request);

                if (response.Items.Count == 0)
                    return ServiceResult<User>.Failure("Usuário não encontrado");

                var user = MapDynamoDBItemToUser(response.Items[0]);
                return ServiceResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failure($"Erro ao buscar usuário: {ex.Message}");
            }
        }

        public async Task<ServiceResult<User>> CreateAsync(User user)
        {
            try
            {
                var item = MapUserToDynamoDBItem(user);

                var request = new PutItemRequest
                {
                    TableName = _tableName,
                    Item = item
                };

                await _dynamoDb.PutItemAsync(request);
                return ServiceResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failure($"Erro ao criar usuário: {ex.Message}");
            }
        }

        public async Task<ServiceResult<User>> UpdateAsync(User user)
        {
            try
            {
                var item = MapUserToDynamoDBItem(user);

                var request = new PutItemRequest
                {
                    TableName = _tableName,
                    Item = item
                };

                await _dynamoDb.PutItemAsync(request);
                return ServiceResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failure($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddFavoriteCityAsync(string userId, string city)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["Id"] = new AttributeValue { S = userId }
                    },
                    UpdateExpression = "ADD FavoriteCities :city",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [":city"] = new AttributeValue { SS = new List<string> { city } }
                    }
                };

                await _dynamoDb.UpdateItemAsync(request);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Erro ao adicionar cidade favorita: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveFavoriteCityAsync(string userId, string city)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["Id"] = new AttributeValue { S = userId }
                    },
                    UpdateExpression = "DELETE FavoriteCities :city",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [":city"] = new AttributeValue { SS = new List<string> { city } }
                    }
                };

                await _dynamoDb.UpdateItemAsync(request);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Erro ao remover cidade favorita: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<string>>> GetFavoriteCitiesAsync(string userId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (!user.IsSuccess)
                    return ServiceResult<List<string>>.Failure(user.Error);

                return ServiceResult<List<string>>.Success(user.Data.FavoriteCities);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<string>>.Failure($"Erro ao buscar cidades favoritas: {ex.Message}");
            }
        }
        public async Task<ServiceResult<bool>> AddFavoriteCountryAsync(string userId, string country)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["Id"] = new AttributeValue { S = userId }
                    },
                    UpdateExpression = "ADD FavoriteCountries :country",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [":country"] = new AttributeValue { SS = new List<string> { country } }
                    }
                };

                await _dynamoDb.UpdateItemAsync(request);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Erro ao adicionar país favorito: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<string>>> GetFavoriteCountriesAsync(string userId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (!user.IsSuccess)
                    return ServiceResult<List<string>>.Failure(user.Error);

                return ServiceResult<List<string>>.Success(user.Data.FavoriteCountries);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<string>>.Failure($"Erro ao buscar países favoritos: {ex.Message}");
            }
        }
        public async Task<ServiceResult<bool>> RemoveFavoriteCountryAsync(string userId, string country)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["Id"] = new AttributeValue { S = userId }
                    },
                    UpdateExpression = "DELETE FavoriteCountries :country",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [":country"] = new AttributeValue { SS = new List<string> { country } }
                    }
                };

                await _dynamoDb.UpdateItemAsync(request);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Erro ao remover país favorito: {ex.Message}");
            }
        }
        private User MapDynamoDBItemToUser(Dictionary<string, AttributeValue> item)
        {
            return new User
            {
                Id = item["Id"].S,
                Email = item["Email"].S,
                PasswordHash = item["PasswordHash"].S,
                CreatedAt = DateTime.Parse(item["CreatedAt"].S),
                FavoriteCities = item.ContainsKey("FavoriteCities") ? item["FavoriteCities"].SS : new List<string>(),
                FavoriteCountries = item.ContainsKey("FavoriteCountries") ? item["FavoriteCountries"].SS : new List<string>()
            };
        }

        private Dictionary<string, AttributeValue> MapUserToDynamoDBItem(User user)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                ["Id"] = new AttributeValue { S = user.Id },
                ["Email"] = new AttributeValue { S = user.Email },
                ["PasswordHash"] = new AttributeValue { S = user.PasswordHash },
                ["CreatedAt"] = new AttributeValue { S = user.CreatedAt.ToString("O") }
            };

            if (user.FavoriteCities != null && user.FavoriteCities.Count > 0)
                item["FavoriteCities"] = new AttributeValue { SS = user.FavoriteCities };

            if (user.FavoriteCountries != null && user.FavoriteCountries.Count > 0)
                item["FavoriteCountries"] = new AttributeValue { SS = user.FavoriteCountries };

            return item;
        }
    }
}
