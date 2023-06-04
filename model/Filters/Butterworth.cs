using MathNet.Filtering.Butterworth;
using MathNet.Filtering.FIR;
using NWaves.Filters.Butterworth;
using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Butterworth : FilterBase
    {
        public Butterworth(FilterConfigurationType filterConfigurationType, int samplingFrequency) : base(filterConfigurationType, samplingFrequency)
        {
            FilterType = FilterType.Butterworth;
        }

        protected override Task InitializeBandPass()
        {
            throw new NotImplementedException();
        }

        protected override Task InitializeBandStop()
        {
            throw new NotImplementedException();
        }

        protected override Task InitializeHighPass()
        {
            throw new NotImplementedException();
        }

        protected override async Task InitializeLowPass()
        {
            await Task.Run(() =>
            {
                var nyquist = SamplingFrequency / 2d;
                var normalizedCutoff = FilterParameters[FilterParameterName.Cutoff].Value / nyquist;

                Coefficients = IirCoefficients.LowPass(normalizedCutoff + TransitionBandwidth, normalizedCutoff - TransitionBandwidth, DefaultPassbandRipple, DefaultStopbandAttenuation);

            });


        }
    }
}
