﻿using System;
using System.Linq;
using VL.Core;
using VL.Xenko.Rendering;
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Xenko.Rendering;

namespace VL.Xenko.EffectLib
{
    class EffectNode : IVLNode, IEffect
    {
        readonly EffectNodeDescription description;
        readonly DynamicEffectInstance instance;
        readonly GraphicsDevice graphicsDevice;
        readonly PerFrameParameters[] perFrameParams;
        readonly PerViewParameters[] perViewParams;
        readonly PerDrawParameters[] perDrawParams;
        readonly ParameterCollection parameters;
        Pin<Action<ParameterCollection, RenderView, RenderDrawContext>> customParameterSetterPin;
        ConvertedValueParameterPin<Matrix, SharpDX.Matrix> worldPin;

        public EffectNode(EffectNodeDescription description)
        {
            this.description = description;
            graphicsDevice = description.Factory.DeviceService.GraphicsDevice;
            instance = new DynamicEffectInstance(description.Name);
            instance.Initialize(description.Factory.ServiceRegistry);
            instance.UpdateEffect(graphicsDevice);
            parameters = instance.Parameters;
            Inputs = description.CreateNodeInputs(this, parameters);
            Outputs = description.CreateNodeOutputs(this, parameters);
            perFrameParams = parameters.GetWellKnownParameters(WellKnownParameters.PerFrameMap).ToArray();
            perViewParams = parameters.GetWellKnownParameters(WellKnownParameters.PerViewMap).ToArray();
            perDrawParams = parameters.GetWellKnownParameters(WellKnownParameters.PerDrawMap).ToArray();
            if (perDrawParams.Length > 0)
                worldPin = Inputs.OfType<ConvertedValueParameterPin<Matrix, SharpDX.Matrix>>().FirstOrDefault(p => p.Key == TransformationKeys.World);
            if (Inputs.Length > 0)
                customParameterSetterPin = Inputs[Inputs.Length - 1] as Pin<Action<ParameterCollection, RenderView, RenderDrawContext>>;
        }

        public IVLPin[] Inputs { get; }

        public IVLPin[] Outputs { get; }

        public void Update()
        {
            if (instance.UpdateEffect(graphicsDevice))
            {
                foreach (var p in Inputs.OfType<ParameterPin>())
                    p.Update(parameters);
            }
        }

        public void Dispose()
        {
            instance.Dispose();
        }

        EffectInstance IEffect.SetParameters(RenderView renderView, RenderDrawContext renderDrawContext)
        {
            try
            {
                // TODO1: PerFrame could be done in Update if we'd have access to frame time
                // TODO2: This code can be optimized by using parameter accessors and not parameter keys
                parameters.SetPerFrameParameters(perFrameParams, renderDrawContext.RenderContext);
                parameters.SetPerViewParameters(perViewParams, renderView);

                if (worldPin != null)
                {
                    var world = worldPin.ShaderValue;
                    parameters.SetPerDrawParameters(perDrawParams, renderView, ref world);
                }

                customParameterSetterPin?.Value?.Invoke(parameters, renderView, renderDrawContext);
            }
            catch (Exception e)
            {
                RuntimeGraph.ReportException(e);
            }
            return instance;
        }
    }
}