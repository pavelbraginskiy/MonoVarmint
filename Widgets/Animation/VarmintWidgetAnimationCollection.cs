﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonoVarmint.Widgets
{
    //--------------------------------------------------------------------------------------
    /// <summary>
    /// MoveOffsetLinear - animate an offset from one value to another
    /// </summary>
    //--------------------------------------------------------------------------------------
    public partial class VarmintWidgetAnimation
    {

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// ForegroundColorFade - Smooth transition from one color to another
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation ForegroundColorFade(
            double durationSeconds,
            Color startColor,
            Color endColor)
        {
            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {
                var deltaVector = (endColor.ToVector4() - startColor.ToVector4()) * (float)delta;
                var newColor = new Color(startColor.ToVector4() + deltaVector);
                widget.ForegroundColor = newColor;
            });
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// RotateConsistent - animate a rotation of a specified amount with a consistent speed
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation RotateLinear(
            double durationSeconds,
            float rotationAmount)
        {
            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {
                widget.Rotate = rotationAmount * (float)delta;
            });
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// ScaleConsistent - animate a scale with a specified factor in relation to the current size with a consistent speed
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation ScaleLinear(
            Vector2 originalSize,
            double durationSeconds,
            float scaleFactor)
        {
            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {

                // Animations scales from 1 (current size) to scale factor
                float changeFactor = 0;
                if (scaleFactor < 0) throw new ArgumentException();
                if (scaleFactor < 1) changeFactor = -scaleFactor;
                if (scaleFactor >= 1) changeFactor = 1 - scaleFactor;

                widget.Size = originalSize * (1 + (changeFactor * (float)delta));

            });
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// MoveOffsetLinear - animate an offset from one value to another in a straight line
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation MoveOffsetLinear(
            double durationSeconds,
            Vector2 startPosition,
            Vector2 endPosition)
        {
            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {
                widget.Offset = startPosition + (endPosition - startPosition) * (float)delta;
            });
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// RotateLinear - animate a rotation from one value to another with linear velocity
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation RotateLinear(
            double durationSeconds,
            float startRotation,
            float endRotation)
        {
            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {
                widget.Rotate = startRotation + (endRotation - startRotation) * (float)delta;
            });
        }
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// MoveOffsetNaturalLinear - animate an offset from one value to another in a 
        ///                           straight line, with some acceleration and deceleration
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation MoveOffsetNaturalLinear(
            double durationSeconds,
            Vector2 startPosition,
            Vector2 endPosition)
        {
            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {
                var acceleratedDelta = (2 - (Math.Cos(delta * Math.PI) + 1)) / 2;
                widget.Offset = startPosition + (endPosition - startPosition) * (float)acceleratedDelta;
            });
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// MoveOffsetAveragedBilinear - animate an offset from one value to another, starting
        ///                              with the original trajectory, but ending with the
        ///                              final trajectory.  This creates a curved path.
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation MoveOffsetAveragedBilinear(
            double durationSeconds,
            Vector2 startPosition,
            Vector2 endPositionStart,
            Vector2 endPositionFinal)
        {
            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {
                var positionStart = startPosition + (endPositionStart - startPosition) * (float)delta;
                var position2 = startPosition + (endPositionFinal - startPosition) * (float)delta;
                widget.Offset = positionStart * (1 - (float)delta) + position2 * (float)delta;
            });
        }


        //--------------------------------------------------------------------------------------
        /// <summary>
        /// MoveOffsetSpiral - Spiral into a location
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation MoveOffsetSpiral(
            double durationSeconds,
            Vector2 startPosition,
            Vector2 endPosition,
            double angularSpeed)
        {
            var deltaVector = endPosition - startPosition;
            var radius = deltaVector.Length();
            var startAngle = Math.Atan2(deltaVector.Y, deltaVector.X);

            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {
                var theta = startAngle + delta * angularSpeed;
                var r = (1 - delta) * radius;
                var x = endPosition.X - r * Math.Cos(theta);
                var y = endPosition.Y - r * Math.Sin(theta);
                widget.Offset = new Vector2((float)x, (float)y);
            });
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// MoveOffsetBounceInfinite
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation MoveOffsetBounceInfinite(
            Vector2 startPosition,
            Vector2 endPosition,
            double rate = 1,
            double offset = 0)
        {
            if (rate < 0 || rate > 1000) throw new ApplicationException("Rate should be between 0 and 1000");
            var travelVector = endPosition - startPosition;
            var x = -0.5 + offset;
            var lastDelta = 0.0;

            return new VarmintWidgetAnimation(0, (widget, delta) =>
            {
                var step = delta - lastDelta;
                lastDelta = delta;
                x += step * rate * 2;
                while(x > 1)
                {
                    x -= 2.0;
                }
                var adjustedDelta =  x * x;

                widget.Offset = startPosition + travelVector * (float)(adjustedDelta);
            });
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// MoveOffsetByProfile
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static VarmintWidgetAnimation MoveOffsetByProfile(
            double durationSeconds,
            Vector2 startPosition,
            Vector2 endPosition,
            double[] profile)
        {
            if (durationSeconds <= 0) throw new ApplicationException("Duration must have a value > 0");
            var travelVector = endPosition - startPosition;

            return new VarmintWidgetAnimation(durationSeconds, (widget, delta) =>
            {
                var index = (int)(delta * (profile.Length - 1));
                var ratio = delta * profile.Length - index;
                if (index >= profile.Length)
                {
                    ratio = 0;
                    index = profile.Length - 1;
                }
                var left = profile[index];
                var right = profile[index];
                if(index < profile.Length -1) right = profile[index + 1];

                var travel = travelVector * (float)(left * (1 - ratio) + right * ratio);             

                widget.Offset = startPosition + travel;
            });
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// GenerateBounceProfile - generates an array of numbers representing a bounce movement
        /// 
        /// bounce      # of bounces in the profile (stops on the last bounce)
        /// decay       # 0.0 - 1.0 how much to decay bounce height on each bounce
        /// start       # 0 = start at the bounce, 0.5 = start at the top
        /// </summary>
        //--------------------------------------------------------------------------------------
        public static double[] GenerateBounceProfile(int bounces, double decay, double start)
        {
            var output = new List<double>();

            var x = -0.5 + start;
            var extent = 1.0;
            var bounceCount = 0;
            var step = 0.02;

            while(bounceCount < bounces)
            {
                var adjustedDelta = 1 - ((1 - x * x) * extent);
                output.Add(adjustedDelta);
                x += step;
                while (x > 1)
                {
                    x -= 2.0;
                    extent *= (1 - decay);
                    bounceCount++;
                    step /= (1 - decay * decay);
                }
            }

            output.Add(1.0);

            return output.ToArray();
        }

    }
}
