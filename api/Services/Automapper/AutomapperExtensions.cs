using AutoMapper;
using Domain;
using System.Collections.Generic;

namespace Services
{
	public static class AutomapperExtensions
	{

		public static T MapTo<T>(this BaseEntity entity)
		{
			return Mapper.Map<T>(entity);
		}

		public static T MapTo<T>(this IEnumerable<BaseEntity> entities)
		{
			return Mapper.Map<T>(entities);
		}


        public static T MapTo<T>(this UserRegistrationModel userRegistrationModel)
        {
            return Mapper.Map<T>(userRegistrationModel);
        }

    }
}
