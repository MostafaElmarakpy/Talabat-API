﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Data;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();



        public async Task<T> GetByIdAsync(int id)
        //=> await _context.Set<T>().Where(item => item.Id == id).FirstOrDefaultAsync();
        => await _context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }
        public async Task<T> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }


        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec);
        }

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task CreateAsync(T entity)
            => await _context.Set<T>().AddAsync(entity);

        public void Update(T entity)
            => _context.Set<T>().Update(entity);

        public void Delete(T entity)
       => _context.Set<T>().Remove(entity);
    }
}
