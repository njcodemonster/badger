﻿using GenericModals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Helpers
{
    public class ValidationProblemDetailsResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
            var errors = new List<ValidationError>();

            var details = "See ValidationErrors for details";

            if (modelStateEntries.Any())
            {
                if (modelStateEntries.Length == 1 && modelStateEntries[0].Value.Errors.Count == 1 && modelStateEntries[0].Key == string.Empty)
            {
                    details = modelStateEntries[0].Value.Errors[0].ErrorMessage;
                }
            else
                {
                    foreach (var modelStateEntry in modelStateEntries)
                    {
                        foreach (var modelStateError in modelStateEntry.Value.Errors)
                        {
                            var error = new ValidationError
                            {
                                Name = modelStateEntry.Key,
                                Description = modelStateError.ErrorMessage
                            };

                            errors.Add(error);
                        }
                    }
                }
            }

            var problemDetails = new ResponseModel
            {
                Status = System.Net.HttpStatusCode.BadRequest,
                Message = JsonConvert.SerializeObject(errors) + "Details: " + details
            };

            context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
            return Task.CompletedTask;
        }
    }
}
