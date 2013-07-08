using System;
using Ddd.Template.Contracts;

namespace Ddd.Template.Domain
{
	public interface IRepository<out T> where T : AggregateRoot, new()
	{
		void Save(Guid commitId, AggregateRoot aggregate, int originalVersion, Guid originatorId);
		T GetById(Guid id);
		bool Exists(Guid id);
	}
}
