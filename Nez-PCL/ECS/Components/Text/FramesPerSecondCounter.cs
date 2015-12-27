﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.BitmapFonts;


namespace Nez
{
	// TODO: currently, docking doesnt take into account ViewportAdapters. it also doesn't update position when screen size changes
	public class FramesPerSecondCounter : Text
	{
		public enum FPSDockPosition
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}

		public long totalFrames;
		public float averageFramesPerSecond;
		public float currentFramesPerSecond;
		public int maximumSamples;
		public FPSDockPosition dockPosition;

		readonly Queue<float> _sampleBuffer = new Queue<float>();


		public FramesPerSecondCounter( BitmapFont font, Color color, FPSDockPosition dockPosition = FPSDockPosition.TopRight, int maximumSamples = 100 ) : base( font, string.Empty, Vector2.Zero, color )
		{
			this.maximumSamples = maximumSamples;
			this.dockPosition = dockPosition;
			initialize();
		}


		public FramesPerSecondCounter( SpriteFont font, Color color, FPSDockPosition dockPosition = FPSDockPosition.TopRight, int maximumSamples = 100 ) : base( font, string.Empty, Vector2.Zero, color )
		{
			this.maximumSamples = maximumSamples;
			this.dockPosition = dockPosition;
			initialize();
		}


		void initialize()
		{
			switch( dockPosition )
			{
				case FPSDockPosition.TopLeft:
					_horizontalAlign = HorizontalAlign.Left;
				break;
				case FPSDockPosition.TopRight:
					_horizontalAlign = HorizontalAlign.Right;
					localPosition = new Vector2( Core.graphicsDevice.Viewport.Width, 0f );
				break;
				case FPSDockPosition.BottomLeft:
					_horizontalAlign = HorizontalAlign.Left;
					_verticalAlign = VerticalAlign.Bottom;
					localPosition = new Vector2( 0, Core.graphicsDevice.Viewport.Height );
				break;
				case FPSDockPosition.BottomRight:
					_horizontalAlign = HorizontalAlign.Right;
					_verticalAlign = VerticalAlign.Bottom;
					localPosition = new Vector2( Core.graphicsDevice.Viewport.Width, Core.graphicsDevice.Viewport.Height );
				break;
			}
		}


		public void reset()
		{
			totalFrames = 0;
			_sampleBuffer.Clear();
		}


		public override void update()
		{
			currentFramesPerSecond = 1.0f / Time.unscaledDeltaTime;
			_sampleBuffer.Enqueue( currentFramesPerSecond );

			if( _sampleBuffer.Count > maximumSamples )
			{
				_sampleBuffer.Dequeue();
				averageFramesPerSecond = _sampleBuffer.Average( i => i );
			}
			else
			{
				averageFramesPerSecond = currentFramesPerSecond;
			}

			totalFrames++;

			text = string.Format( "FPS: {0:0.00}", averageFramesPerSecond );
		}


		public override void render( Graphics graphics, Camera camera )
		{
			// we override render and use position instead of entityPosition. this keeps the text in place even if the entity moves
			if( _bitmapFont != null )
				graphics.spriteBatch.DrawString( _bitmapFont, _text, localPosition, color, rotation, origin, scale, spriteEffects, layerDepth );
			else
				graphics.spriteBatch.DrawString( _spriteFont, _text, localPosition, color, rotation, origin, scale, spriteEffects, layerDepth );
		}


		public override void debugRender( Graphics graphics )
		{
			// due to the override of position in render we have to do the same here
			var rect = bounds;
			rect.Location = localPosition.ToPoint();
			graphics.spriteBatch.drawHollowRect( rect, Color.Yellow );
		}

	}
}
