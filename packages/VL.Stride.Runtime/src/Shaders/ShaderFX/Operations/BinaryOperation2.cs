using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Stride.Core;
using Stride.Rendering.Materials;
using Stride.Shaders;
using DataMemberAttribute = Stride.Core.DataMemberAttribute;
using static VL.Stride.Shaders.ShaderFX.ShaderFXUtils;


namespace VL.Stride.Shaders.ShaderFX
{
    public class BinaryOperation2<TIn, TOut> : ComputeValue<TOut>
    {
        public BinaryOperation2(string operatorName, IComputeValue<TIn> left, IComputeValue<TIn> right)
        {
            ShaderName = operatorName;
            Left = left;
            Right = right;
        }

        public IComputeValue<TIn> Left { get; }

        public IComputeValue<TIn> Right { get; }

        public override IEnumerable<IComputeNode> GetChildren(object context = null)
        {
            return ReturnIfNotNull(Left, Right);
        }

        public override ShaderSource GenerateShaderSource(ShaderGeneratorContext context, MaterialComputeColorKeys baseKeys)
        {
            var shaderSource = GetShaderSourceForType<TIn>(ShaderName);

            var mixin = shaderSource.CreateMixin();

            mixin.AddComposition(Left,"Left", context, baseKeys);
            mixin.AddComposition(Right, "Right", context, baseKeys);

            return mixin;
        }

        public override string ToString()
        {
            return string.Format("Op {0} {1} {2}", Left, ShaderName, Right);
        }
    }
}
