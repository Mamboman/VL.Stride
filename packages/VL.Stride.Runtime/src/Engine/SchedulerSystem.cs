﻿using Stride.Core;
using Stride.Core.Annotations;
using Stride.Games;
using Stride.Graphics;
using Stride.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace VL.Stride.Engine
{
    /// <summary>
    /// Allows to schedule game systems (e.g. a SceneSystem or a LayerSystem) as well as layers.
    /// </summary>
    public class SchedulerSystem : GameSystemBase
    {
        static readonly PropertyKey<bool> contentLoaded = new PropertyKey<bool>("ContentLoaded", typeof(SchedulerSystem));

        readonly List<GameSystemBase> queue = new List<GameSystemBase>();
        readonly Stack<ConsecutiveLayerSystem> pool = new Stack<ConsecutiveLayerSystem>();

        public SchedulerSystem([NotNull] IServiceRegistry registry) : base(registry)
        {
            Enabled = true;
            Visible = true;
        }

        /// <summary>
        /// Schedule a game system to be processed in this frame.
        /// </summary>
        /// <param name="gameSystem">The game system to schedule.</param>
        public void Schedule(GameSystemBase gameSystem)
        {
            queue.Add(gameSystem);
        }

        /// <summary>
        /// Schedules a layer for rendering.
        /// </summary>
        /// <param name="layer">The layer to schedule.</param>
        public void Schedule(IGraphicsRendererBase layer)
        {
            var current = queue.LastOrDefault() as ConsecutiveLayerSystem;
            if (current is null)
            {
                // Fetch from pool or create new
                current = pool.Count > 0 ? pool.Pop() : new ConsecutiveLayerSystem(Services);
                Schedule(current);
            }
            current.Layers.Add(layer);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var system in queue)
            {
                if (!system.Tags.Get(contentLoaded) && system is IContentable c)
                {
                    c.LoadContent();
                    system.Tags.Set(contentLoaded, true);
                }
                if (system.Enabled)
                    system.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            try
            {
                foreach (var s in queue)
                {
                    if (s.Visible)
                        s.Draw(gameTime);
                }
            }
            finally
            {
                // Put back into the pool
                foreach (var s in queue)
                    if (s is ConsecutiveLayerSystem c)
                        pool.Push(c);

                queue.Clear();
            }
        }

        sealed class ConsecutiveLayerSystem : GameSystemBase
        {
            private RenderView renderView;
            private RenderContext renderContext;
            private RenderDrawContext renderDrawContext;

            public ConsecutiveLayerSystem([NotNull] IServiceRegistry registry) : base(registry)
            {
                Enabled = true;
                Visible = true;
            }

            public readonly List<IGraphicsRendererBase> Layers = new List<IGraphicsRendererBase>();

            protected override void LoadContent()
            {
                // Default render view
                renderView = new RenderView()
                {
                    NearClipPlane = 0.05f,
                    FarClipPlane = 1000,
                };

                // Create the drawing context
                var graphicsContext = Services.GetSafeServiceAs<GraphicsContext>();
                renderContext = RenderContext.GetShared(Services);
                renderDrawContext = new RenderDrawContext(Services, renderContext, graphicsContext);
            }

            public override void Draw(GameTime gameTime)
            {
                try
                {
                    using (renderContext.PushRenderViewAndRestore(renderView))
                    using (renderDrawContext.PushRenderTargetsAndRestore())
                    {
                        foreach (var layer in Layers)
                            layer.Draw(renderDrawContext);
                    }
                }
                finally
                {
                    Layers.Clear();
                }
            }
        }
    }
}