using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Faktori.EasingFunctions {
    public static partial class Easing {
        public static float Interpolate(float value, Func<float, float> easing) => easing(value);
        public static float Interpolate(float value, Functions easing) => Interpolate(value, GetFunction(easing));
        public static Func<float, float> GetFunction(Functions easing) => _functionsMap[easing];
        public enum Functions {
            Linear,
            QuadraticIn,
            QuadraticOut,
            QuadraticInOut,
            CubicIn,
            CubicOut,
            CubicInOut,
            QuarticIn,
            QuarticOut,
            QuarticInOut,
            QuinticIn,
            QuinticOut,
            QuinticInOut,
            SinusoidalIn,
            SinusoidalOut,
            SinusoidalInOut,
            ExponentialIn,
            ExponentialOut,
            ExponentialInOut,
            CircularIn,
            CircularOut,
            CircularInOut,
            ElasticIn,
            ElasticOut,
            ElasticInOut,
            BackIn,
            BackOut,
            BackInOut,
            BounceIn,
            BounceOut,
            BounceInOut
        }

        private static Dictionary<Functions, Func<float, float>> _functionsMap = new Dictionary<Functions, Func<float, float>> {
            { Functions.Linear, Linear },

            { Functions.QuadraticIn,    Quadratic.In },
            { Functions.QuadraticOut,   Quadratic.Out },
            { Functions.QuadraticInOut, Quadratic.InOut },
            
            { Functions.CubicIn,    Cubic.In },
            { Functions.CubicOut,   Cubic.Out },
            { Functions.CubicInOut, Cubic.InOut },
            
            { Functions.QuarticIn,    Quartic.In },
            { Functions.QuarticOut,   Quartic.Out },
            { Functions.QuarticInOut, Quartic.InOut },
            
            { Functions.QuinticIn,    Quintic.In },
            { Functions.QuinticOut,   Quintic.Out },
            { Functions.QuinticInOut, Quintic.InOut },
            
            { Functions.SinusoidalIn,    Sinusoidal.In },
            { Functions.SinusoidalOut,   Sinusoidal.Out },
            { Functions.SinusoidalInOut, Sinusoidal.InOut },
            
            { Functions.ExponentialIn,    Exponential.In },
            { Functions.ExponentialOut,   Exponential.Out },
            { Functions.ExponentialInOut, Exponential.InOut },
            
            { Functions.CircularIn,    Circular.In },
            { Functions.CircularOut,   Circular.Out },
            { Functions.CircularInOut, Circular.InOut },
            
            { Functions.ElasticIn,    Elastic.In },
            { Functions.ElasticOut,   Elastic.Out },
            { Functions.ElasticInOut, Elastic.InOut },
            
            { Functions.BackIn,    Back.In },
            { Functions.BackOut,   Back.Out },
            { Functions.BackInOut, Back.InOut },
            
            { Functions.BounceIn,    Bounce.In },
            { Functions.BounceOut,   Bounce.Out },
            { Functions.BounceInOut, Bounce.InOut },
        };
        public static float Linear (float k) => k;
        public class Quadratic
        {
            public static float In (float k) => k*k;
            public static float Out (float k) => k*(2f - k);
            public static float InOut (float k) => ((k *= 2f) < 1f) ? 0.5f*k*k : -0.5f*((k -= 1f)*(k - 2f) - 1f);
        }

        public class Cubic
        {
            public static float In (float k) => k*k*k;
            public static float Out (float k) => 1f + ((k -= 1f)*k*k);
            public static float InOut (float k) => ((k *= 2f) < 1f) ? 0.5f*k*k*k : 0.5f*((k -= 2f)*k*k + 2f);
        }

        public class Quartic 
        {
            public static float In (float k) => k*k*k*k;
            public static float Out (float k) => 1f - ((k -= 1f)*k*k*k);
            public static float InOut (float k) => ((k *= 2f) < 1f) ? 0.5f*k*k*k*k : -0.5f*((k -= 2f)*k*k*k - 2f);
        }

        public class Quintic
        {		
            public static float In (float k) => k*k*k*k*k;
            public static float Out (float k) => 1f + ((k -= 1f)*k*k*k*k);
            public static float InOut (float k) => ((k *= 2f) < 1f) ? 0.5f*k*k*k*k*k : 0.5f*((k -= 2f)*k*k*k*k + 2f);
        }

        public class Sinusoidal
        {		
            public static float In (float k) => 1f - Mathf.Cos(k*Mathf.PI/2f);
            public static float Out (float k) => Mathf.Sin(k*Mathf.PI/2f);
            public static float InOut (float k) => 0.5f*(1f - Mathf.Cos(Mathf.PI*k));
        }

        public class Exponential
        {		
            public static float In (float k) => k == 0f? 0f : Mathf.Pow(1024f, k - 1f);
            public static float Out (float k) => k == 1f? 1f : 1f - Mathf.Pow(2f, -10f*k);
            public static float InOut (float k) {
                if (k == 0f) return 0f;
                if (k == 1f) return 1f;
                if ((k *= 2f) < 1f) return 0.5f*Mathf.Pow(1024f, k - 1f);
                return 0.5f*(-Mathf.Pow(2f, -10f*(k - 1f)) + 2f);
            }
        }

        public class Circular
        {		
            public static float In (float k) => 1f - Mathf.Sqrt(1f - k*k);		
            public static float Out (float k) => Mathf.Sqrt(1f - ((k -= 1f)*k));
            public static float InOut (float k) => ((k *= 2f) < 1f) ? -0.5f*(Mathf.Sqrt(1f - k*k) - 1) :  0.5f*(Mathf.Sqrt(1f - (k -= 2f)*k) + 1f);
        }

        public class Elastic
        {
            public static float In (float k) {
                if (k == 0) return 0;
                if (k == 1) return 1;
                return -Mathf.Pow( 2f, 10f*(k -= 1f))*Mathf.Sin((k - 0.1f)*(2f*Mathf.PI)/0.4f);
            }
            
            public static float Out (float k) {
                if (k == 0) return 0;
                if (k == 1) return 1;
                return Mathf.Pow(2f, -10f*k)*Mathf.Sin((k - 0.1f)*(2f*Mathf.PI)/0.4f) + 1f;
            }
            
            public static float InOut (float k) => ((k *= 2f) < 1f) 
                ? -0.5f*Mathf.Pow(2f, 10f*(k -= 1f))*Mathf.Sin((k - 0.1f)*(2f*Mathf.PI)/0.4f)
                : Mathf.Pow(2f, -10f*(k -= 1f))*Mathf.Sin((k - 0.1f)*(2f*Mathf.PI)/0.4f)*0.5f + 1f;
        }

        public class Back
        {
            static float s = 1.70158f;
            static float s2 = 2.5949095f;
            public static float In (float k) => k*k*((s + 1f)*k - s);
            public static float Out (float k) => (k -= 1f)*k*((s + 1f)*k + s) + 1f;
            public static float InOut (float k) => ((k *= 2f) < 1f) ? 0.5f*(k*k*((s2 + 1f)*k - s2)) : 0.5f*((k -= 2f)*k*((s2 + 1f)*k + s2) + 2f);
        }

        public class Bounce
        {		
            public static float In (float k) => 1f - Out(1f - k);
            public static float Out (float k) {			
                if (k < (1f/2.75f)) {
                    return 7.5625f*k*k;				
                }
                else if (k < (2f/2.75f)) {
                    return 7.5625f*(k -= (1.5f/2.75f))*k + 0.75f;
                }
                else if (k < (2.5f/2.75f)) {
                    return 7.5625f *(k -= (2.25f/2.75f))*k + 0.9375f;
                }
                else {
                    return 7.5625f*(k -= (2.625f/2.75f))*k + 0.984375f;
                }
            }
            public static float InOut (float k) => (k < 0.5f) ? In(k*2f)*0.5f : Out(k*2f - 1f)*0.5f + 0.5f;
        }
    }
}