using DefectTracker.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DefectTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefectTrackerController : ControllerBase
    {
        private readonly DefecttrackerContext _context;
        public DefectTrackerController(DefecttrackerContext context)
        {
            _context = context;
        }


        // Only an Administrator can query.
        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public object Get()

        {
            StringValues Skip;
            StringValues Take;
            StringValues OrderBy;

            // Filter the data.
            var TotalRecordCount = _context.Defects.Count();
            int skip = (Request.Query.TryGetValue("$skip", out Skip))
            ? Convert.ToInt32(Skip[0]) : 0;
            int top = (Request.Query.TryGetValue("$top", out Take))
            ? Convert.ToInt32(Take[0]) : TotalRecordCount;
            string orderby =
            (Request.Query.TryGetValue("$orderby", out OrderBy))
            ? OrderBy.ToString() : "TicketDate";

            // Handle OrderBy direction.

            if (orderby.EndsWith(" desc"))
            {
                orderby = orderby.Replace(" desc", "");

                return new
                {
                    Items = _context.Defects
                    .OrderByDescending(orderby)
                    .Skip(skip)
                    .Take(top),
                    Count = TotalRecordCount
                };
            }
            else
            {
                _ =
                typeof(Defects).GetProperty(orderby);
                return new
                {
                    Items = _context.Defects
               .OrderBy(orderby)
               .Skip(skip)
               .Take(top),
                    Count = TotalRecordCount
                };
            }
        }
        [HttpPost]

        [AllowAnonymous]
        public Task
        Post(Defects newDefects)
        {
            // Add a new Defect.

            _context.Defects.Add(newDefects);
            _context.SaveChanges();
            return Task.FromResult(newDefects);
        }
        [HttpPut]
        [AllowAnonymous]
        public Task
        PutAsync(Defects UpdatedDefects)
        {
            // Get the existing record.
            // Note: Caller must have the DefectGuid.

            var ExistingDefect =  _context.Defects
            .Where(x => x.DefectGuid == UpdatedDefects.DefectGuid)
            .FirstOrDefault();

            if (ExistingDefect != null)
            {
                ExistingDefect.DefectAt =
                UpdatedDefects.DefectAt;

                ExistingDefect.DefectDescription =
                UpdatedDefects.DefectDescription;

                ExistingDefect.DefectGuid =
                UpdatedDefects.DefectGuid;

                ExistingDefect.DefectReporterEmail =
                UpdatedDefects.DefectReporterEmail;

                ExistingDefect.DefectStatus =
                UpdatedDefects.DefectStatus;

                // Insert any new DefectDetails.
                if (UpdatedDefects.DefectDetails != null)
                {
                    foreach (var item in UpdatedDefects.DefectDetails)
                    {
                        if (item.Id == 0)
                        {
                            // Create new Defect Details record.

                            DefectDetails newDefectDetails = new DefectDetails();
                            newDefectDetails.DefectId = UpdatedDefects.Id;
                            newDefectDetails.DefectAt = DateTime.Now;
                            newDefectDetails.DefectDescription = item.DefectDescription;

                            _context.DefectDetails
                            .Add(newDefectDetails);
                        }
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);

        }
        // Only an Administrator can delete.
        [Authorize(Roles = "Administrators")]
        [HttpDelete]
        public Task  Delete(string DefectGuid)
        {
            // Get the existing record.
            var ExistingDefect = _context.Defects
            .Include(x => x.DefectDetails)
            .Where(x => x.DefectGuid == DefectGuid)
            .FirstOrDefault();

            if (ExistingDefect != null)
            {

             // Delete the Defect.
                _context.Defects.Remove(ExistingDefect);
                _context.SaveChanges();
            }
            else
            {

                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
    // From: https://bit.ly/30ypMCp
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(
        this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(
        this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }
        private static Expression<Func<T, object>> ToLambda<T>(
        string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property,
           typeof(object));
            return Expression.Lambda<Func<T, object>>(propAsObject,
           parameter);
        }
    }
}



            
