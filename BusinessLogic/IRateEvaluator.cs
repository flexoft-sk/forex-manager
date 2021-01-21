﻿using Flexoft.ForexManager.RatesFetcher;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.BusinessLogic
{
	public interface IRateEvaluator
	{
		Task EvaluateRateAsync(Currency from, Currency to);
	}
}