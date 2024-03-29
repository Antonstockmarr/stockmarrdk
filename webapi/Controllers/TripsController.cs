﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockmarrdk_api.Common;
using stockmarrdk_api.Dto;
using stockmarrdk_api.Models;
using stockmarrdk_api.Services;

namespace stockmarrdk_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet()]
        [Authorize("Users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Trip>))]
        public IActionResult GetTrips()
        {
            List<Trip> trips = _tripService.GetAllTrips();

            return StatusCode(StatusCodes.Status200OK, trips);
        }

        [HttpGet("{year}")]
        [Authorize("Users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Trip))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTrip(int year)
        {
            Trip? trip = _tripService.GetTrip(year);

            if (trip is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Trip from year {year} was not found");
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, trip);
            }
        }

        [HttpPost]
        [Authorize("Admins")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Trip))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult PostTrip([FromForm] NewTripDto newTrip)
        {
            try
            {
                Trip trip = _tripService.PostTrip(newTrip);
                return StatusCode(StatusCodes.Status200OK, trip);
            }
            catch (AlreadyExistsException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPatch]
        [Authorize("Admins")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Trip))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult PatchTrip([FromForm] EditTripDto trip)
        {
            try
            {
                Trip? updatedTrip = _tripService.PatchTrip(trip);
                if (updatedTrip == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, updatedTrip);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{year}")]
        [Authorize("Admins")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Trip))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult DeleteTrip(int year)
        {
            try
            {
                Trip? deletedTrip = _tripService.DeleteTrip(year);
                if (deletedTrip == null)
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, deletedTrip);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
