//using System;
//using System.Runtime.CompilerServices;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.Graphics.Effects;
//
//namespace MonoGame.Extended.Graphics
//{
//    public abstract class Renderer : IDisposable
//    {

//

//
//        /// <summary>
//        ///     Gets or sets the <see cref="Microsoft.Xna.Framework.Graphics.BlendState" /> associated with this
//        ///     <see cref="DynamicBatchRenderer2D" />.
//        /// </summary>
//        /// <value>
//        ///     The <see cref="Microsoft.Xna.Framework.Graphics.BlendState" /> associated with this
//        ///     <see cref="DynamicBatchRenderer2D" />.
//        /// </value>
//        /// <exception cref="InvalidOperationException">
//        ///     <see cref="BlendState" /> cannot be set until <see cref="End" /> has been invoked.
//        /// </exception>
//        public BlendState BlendState
//        {
//            get { return _blendState; }
//            set
//            {
//                EnsureHasNotBegun();
//                _blendState = value;
//            }
//        }
//
//        /// <summary>
//        ///     Gets or sets the <see cref="Microsoft.Xna.Framework.Graphics.DepthStencilState" /> associated with this
//        ///     <see cref="DynamicBatchRenderer2D" />.
//        /// </summary>
//        /// <value>
//        ///     The <see cref="Microsoft.Xna.Framework.Graphics.DepthStencilState" /> associated with this
//        ///     <see cref="DynamicBatchRenderer2D" />.
//        /// </value>
//        /// <exception cref="InvalidOperationException">
//        ///     <see cref="DepthStencilState" /> cannot be set until <see cref="End" /> has been invoked.
//        /// </exception>
//        public DepthStencilState DepthStencilState
//        {
//            get { return _depthStencilState; }
//            set
//            {
//                EnsureHasNotBegun();
//                _depthStencilState = value;
//            }
//        }
//
//        /// <summary>
//        ///     Gets or sets the <see cref="Microsoft.Xna.Framework.Graphics.RasterizerState" /> associated with this
//        ///     <see cref="DynamicBatchRenderer2D" />.
//        /// </summary>
//        /// <value>
//        ///     The <see cref="Microsoft.Xna.Framework.Graphics.RasterizerState" /> associated with this
//        ///     <see cref="DynamicBatchRenderer2D" />.
//        /// </value>
//        /// <exception cref="InvalidOperationException">
//        ///     <see cref="RasterizerState" /> cannot be set until <see cref="End" /> has been invoked.
//        /// </exception>
//        public RasterizerState RasterizerState
//        {
//            get { return _rasterizerState; }
//            set
//            {
//                EnsureHasNotBegun();
//                _rasterizerState = value;
//            }
//        }
//
//        /// <summary>
//        ///     Gets or sets the <see cref="Microsoft.Xna.Framework.Graphics.Effect" /> associated with this
//        ///     <see cref="Renderer" />.
//        /// </summary>
//        /// <value>
//        ///     The <see cref="Microsoft.Xna.Framework.Graphics.Effect" /> associated with this
//        ///     <see cref="Renderer" />.
//        /// </value>
//        /// <exception cref="InvalidOperationException">
//        ///     <see cref="Effect" /> cannot be set until <see cref="End" /> has been invoked.
//        /// </exception>
//        public Effect Effect
//        {
//            get { return _currentEffect; }
//            set
//            {
//                EnsureHasNotBegun();
//                var oldEffect = _currentEffect;
//                _currentEffect = value ?? _defaultEffect;
//                _matrixChainEffect = _currentEffect as IMatrixChainEffect;
//                _effectMatrices = _currentEffect as IEffectMatrices;
//                OnEffetChanged(oldEffect, _currentEffect);
//            }
//        }
//
//        /// <summary>
//        ///     Gets the world-to-view transformation matrix associated with this <see cref="Renderer" />.
//        /// </summary>
//        /// <value>
//        ///     The world-to-view transformation matrix associated with this <see cref="Renderer" />.
//        /// </value>
//        /// <exception cref="InvalidOperationException">
//        ///     <see cref="Effect" /> cannot be set until <see cref="End" /> has been invoked.
//        /// </exception>
//        public Matrix View => _view;
//
//        /// <summary>
//        ///     Gets the view-to-projection transformation matrix associated with this <see cref="Renderer" />.
//        /// </summary>
//        /// <value>
//        ///     The view-to-projection matrix associated with this <see cref="Renderer" />.
//        /// </value>
//        /// <exception cref="InvalidOperationException">
//        ///     <see cref="Effect" /> cannot be set until <see cref="End" /> has been invoked.
//        /// </exception>
//        public Matrix Projection => _projection;
//
//        /// <summary>
//        ///     Gets a value indicating whether batching is currently in progress by being within a <see cref="Begin" /> and
//        ///     <see cref="End" /> pair block of code.
//        /// </summary>
//        /// <value>
//        ///     <c>true</c> if batching has begun; otherwise, <c>false</c>.
//        /// </value>
//        public bool HasBegun { get; internal set; }
//
//        /// <summary>
//        ///     Initializes a new instance of the <see cref="Renderer" /> class.
//        /// </summary>
//        /// <param name="graphicsDevice">The graphics device.</param>
//        /// <param name="defaultEffect">The defaultEffect.</param>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="graphicsDevice" />, or <paramref name="defaultEffect" /> is
//        ///     null.
//        /// </exception>
//        protected Renderer(GraphicsDevice graphicsDevice, Effect defaultEffect)
//        {
//            if (graphicsDevice == null)
//                throw new ArgumentNullException(nameof(graphicsDevice));
//
//            if (defaultEffect == null)
//                throw new ArgumentNullException(nameof(defaultEffect));
//
//            GraphicsDevice = graphicsDevice;
//            BlendState = BlendState.AlphaBlend;
//            DepthStencilState = DepthStencilState.None;
//            RasterizerState = RasterizerState.CullCounterClockwise;
//            Effect = _defaultEffect = defaultEffect;
//            _view = Matrix.Identity;
//            _projection = Matrix.Identity;
//        }
//

//
//

//
//        /// <summary>
//        ///     Starts the rendering pass.
//        /// </summary>
//        /// <param name="viewMatrix">The world-to-view transformation matrix.</param>
//        /// <param name="projectionMatrix">The view-to-projection transformation matrix.</param>
//        /// <exception cref="InvalidOperationException">
//        ///     <see cref="Begin" /> cannot be invoked again until <see cref="End" /> has been invoked.
//        /// </exception>
//        /// <remarks>
//        ///     <para>
//        ///         This method must be called before any enqueuing of draw calls. When all the geometry have been enqueued for
//        ///         drawing, call <see cref="End" />.
//        ///     </para>
//        /// </remarks>
//        public virtual void Begin(ref Matrix viewMatrix, ref Matrix projectionMatrix)
//        {
//            EnsureHasNotBegun();
//            HasBegun = true;
//            _view = viewMatrix;
//            _projection = projectionMatrix;
//        }
//
//        /// <summary>
//        ///     Ends the rendering pass.
//        /// </summary>
//        /// <exception cref="InvalidOperationException">
//        ///     <see cref="End" /> cannot be invoked until <see cref="Begin" /> has been invoked.
//        /// </exception>
//        /// <remarks>
//        ///     <para>
//        ///         This method must be called after all enqueuing of draw calls.
//        ///     </para>
//        /// </remarks>
//        public virtual void End()
//        {
//            EnsureHasBegun();
//            ApplyStates();
//            HasBegun = false;
//        }
//
//        protected virtual void ApplyStates()
//        {
//            if (_matrixChainEffect != null)
//            {
//                _matrixChainEffect.SetProjection(ref _projection);
//                _matrixChainEffect.SetView(ref _view);
//            }
//            else if (_effectMatrices != null)
//            {
//                _effectMatrices.Projection = _projection;
//                _effectMatrices.View = _view;
//            }
//
//            GraphicsDevice.BlendState = _blendState;
//            GraphicsDevice.DepthStencilState = _depthStencilState;
//            GraphicsDevice.RasterizerState = _rasterizerState;
//        }
//
//        protected virtual void OnEffetChanged(Effect oldEffect, Effect currentEffect)
//        {
//        }
//    }
//}
