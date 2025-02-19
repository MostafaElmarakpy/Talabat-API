using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Service;
using Talabat.Core.Specifications.ProductSpecifications;
using Talabat.Dtos;
using Talabat.Errors;
using Talabat.Helpers;

namespace Talabat.Controllers
{
    public class ProductsController
        (

             IUnitOfWork unitOfWork
            , IMapper mapper, IProductService productService

        ) : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IProductService _productService = productService;

        [CachedAttribute(600)]
        [HttpGet]
        public async Task<ActionResult<Pegination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams productParams)
        {
            var products = await _productService.GetProductsAsync(productParams);
            var count = await _productService.GetCountAsync(productParams);
            var Data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(new Pegination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, count, Data));
        }

        [HttpGet("{id}")]

        public async Task<ActionResult> GetProductById(int id)
        {
            var spec = new ProductWithBranAndTypeSpec(id);
            var products = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);

            if (products == null) return NotFound(new ApiResponse(404));
            return Ok(_mapper.Map<Product, ProductToReturnDto>(products));
        }
        [CachedAttribute(3600)]
        [HttpGet("brans")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrans()
        {
            var brans = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brans);

        }
        [CachedAttribute(3600)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(types);

        }
    }
}