﻿using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Butterworth : FilterBase
    {
        public Butterworth(FilterConfigurationType filterConfigurationType, int samplingFrequency) : base(filterConfigurationType, samplingFrequency)
        {
            FilterType = FilterType.Butterworth;
        }

        public override async Task InitializeData()
        {
            await Task.Run(() =>
            {
                FilterData = new double[SamplingFrequency, 2];
                var cutoff = FilterParameters[0].Value;
                //TODO : implement Butterworth filter
                var random = new Random();
                for (var i = 0; i < SamplingFrequency; i++)
                {
                    FilterData[i, 0] = i;
                    // for now create a sine wave with noise
                    FilterData[i, 1] = Math.Sin(i * 2 * Math.PI * 10 / SamplingFrequency) + random.NextDouble() * 0.1;
                }
            });
        }
    }
}
