using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flexoft.ForexManager.BusinessLogic
{
	public static class BusinessLogicModule
	{
		public static void RegiterBusinessLogic(this IServiceCollection services)
		{
			services.AddSingleton<IRateEvaluator, DefaultRateEvaluator>();
			services.AddSingleton(provider => {
				var config = provider.GetService<IConfiguration>();
				return new RateEvaluatorOptions
				{
					CloseOffsetPercentage = float.Parse(config["Logic:CloseOffsetPercentage"]),
					NotificationTarget = config["Logic:NotificationTarget"],
					OpenHour = int.Parse(config["Logic:OpenHour"]),
					OpenAmount = int.Parse(config["Logic:OpenAmount"])
				};
			});
		}
	}
}
