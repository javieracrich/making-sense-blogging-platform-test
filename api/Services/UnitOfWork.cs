using System;
using System.Threading.Tasks;

namespace Services
{
    public interface IUnitOfWork : IDisposable
	{
		MakingSenseDbContext Context { get; }
		/// <summary>
		/// Saves all pending changes
		/// </summary>
		/// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
		int Commit();
		Task<int> CommitAsync();
	}

	public class UnitOfWork : IUnitOfWork
	{
		public UnitOfWork(MakingSenseDbContext ctx)
		{
			this.Context = ctx;
		}

		public MakingSenseDbContext Context { get; }

		public int Commit()
		{
			return Context.SaveChanges();
		}

		public Task<int> CommitAsync()
		{
			return Context.SaveChangesAsync();
		}

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					Context.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
