using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Common.CommonTypes.Interfaces;
using Common.Services;

namespace Common.GenericRepository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly string _tableName;
		private readonly IDataAccessService _accessService;

		public GenericRepository(IDataAccessService accessService)
		{
			_tableName = this.GetTableName()!;
			if (_tableName == null) throw new Exception("Generic type must have Table attribute");
			_accessService = accessService;
		}

		public virtual async Task<List<T>> GetAllAsync()
		{
			var query = "select * from " + _tableName;
			var result = await _accessService.QueryDataAsync<T, dynamic>(query, null);
			return result.ToList();
		}

		public virtual async Task<T?> GetByIdAsync(int id)
		{
			var keyColumn = this.GetKeyColumnName();
			if (keyColumn == null) throw new NullReferenceException("Model must contain primary key property");
			var query = $"select * from {_tableName} where {keyColumn}=@Id;";
			return await _accessService.QuerySingleRecordAsync<T, dynamic>(query, new { Id = id });
		}

		public virtual async Task CreateAsync(T entity, bool excludeKey = false)
		{
			var columns = this.GetColumns(entity);
			var keyColumn = this.GetKeyColumnName();
			var query = $"insert into {_tableName} ";
			int valuesCount = excludeKey ? columns.Count - 1 : columns.Count;
			string[] cols = new string[valuesCount];
			int i = 0;
			foreach (var col in columns)
			{
				if (excludeKey && col.Key == keyColumn) continue;
				cols[i++] = col.Key;
			}

			query += '(' + string.Join(',', cols) + ") ";
			var props = this.GetPropertyNames(excludeKey);
			query += "Values (" + props + ");";
			await _accessService.ExecuteStatementAsync(query, entity);
		}

		public virtual async Task UpdateAsync(T entity, bool excludeKey = false)
		{
			var columns = this.GetColumns(entity);
			var query = $"update {_tableName} set ";
			var colsCount = excludeKey ? columns.Count - 1 : columns.Count;
			string[] cols = new string[colsCount];
			int i = 0;
			foreach (var prop in GetProperties(excludeKey))
			{
				var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
				string propertyName = prop.Name;
				string columnName = columnAttr.Name;
				cols[i++] = $"{columnName} = @{propertyName}";
			}
			query += string.Join(", ", cols);
			var keyColumnName = this.GetKeyColumnName(); 
			var keyPropertyName = this.GetKeyPropertyName();
			query += $" where {keyColumnName} = @{keyPropertyName}";

			await _accessService.ExecuteStatementAsync(query, entity);
		}

		public virtual async Task DeleteAsync(T entity)
		{
			var keyColumn = this.GetKeyColumnName();
			var keyProperty = this.GetKeyPropertyName();
			var query = $"delete from {_tableName} where {keyColumn} = @{keyProperty}";
			await _accessService.ExecuteStatementAsync(query, entity);
		}

		protected string? GetTableName()
		{
			var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
			return tableAttribute?.Name;
		}

		public Dictionary<string, dynamic?> GetColumns(T entity, bool excludeKey = false)
		{
			Dictionary<string, dynamic?> columns = new Dictionary<string, dynamic?>();
			var properties = entity.GetType().GetProperties().Where(p => p.GetCustomAttribute<ColumnAttribute>() != null);
			foreach (PropertyInfo prop in properties)
			{
				if (excludeKey && prop.GetCustomAttribute<KeyAttribute>() != null) continue;
				var name = prop.GetCustomAttribute<ColumnAttribute>()!.Name!;
				var value = prop.GetValue(entity, null);
				columns.Add(name, value);
			}
			return columns;
		}

		public string? GetKeyColumnName()
		{
			foreach (var prop in typeof(T).GetProperties())
			{
				var attribute = prop.GetCustomAttribute<KeyAttribute>();
				if (attribute != null) return prop.GetCustomAttribute<ColumnAttribute>()?.Name;
			}
			return null;
		}

		public string? GetKeyPropertyName()
		{
			var properties = typeof(T).GetProperties()
				.Where(p => p.GetCustomAttribute<KeyAttribute>() != null);

			if (properties.Any()) return properties.FirstOrDefault().Name;
			return null;
		}

		public dynamic? GetKeyColumnValue(T entity)
		{
			foreach (var prop in typeof(T).GetProperties())
			{
				var attribute = prop.GetCustomAttribute<KeyAttribute>();
				if (attribute != null) return prop.GetValue(entity, null);
			}
			return null;
		}

		public string GetPropertyNames(bool excludeKey = false)
		{
			var properties = typeof(T).GetProperties()
				.Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);
			return string.Join(", ", properties.Select(p => $"@{p.Name}"));
		}

		public IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false)
		{
			return typeof(T).GetProperties()
				.Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);
		}

		public string ParseValue(dynamic value)
		{
			if (value == null) return "null";
			switch (value)
			{
				case string: return $"'{value}'";
				case DateTime date:
					return date.ToUniversalTime().ToString("g");
				default: return value.ToString();
			}
		}
	}
}
