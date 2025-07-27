using System;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Graphics.Animation;
using Gum.Managers;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.UI;

/// <summary>
/// A custom button implementation that inherits from Gum's Button class to provide
/// animated visual feedback when focused.
/// </summary>
internal class AnimatedButton : Button
{
  /// <summary>
  /// Creates a new AnimatedButton instance using graphics from the specified texture atlas.
  /// </summary>
  /// <param name="atlas">The texture atlas containing button graphics and animations</param>
  public AnimatedButton(TextureAtlas atlas)
  {
    // Create the top-level container that will hold all visual elements
    // Width is relative to children with extra padding, height is fixed
    var topLevelContainer = new ContainerRuntime
    {
      Height = 14f,
      HeightUnits = DimensionUnitType.Absolute,
      Width = 21f,
      WidthUnits = DimensionUnitType.RelativeToChildren
    };

    // Create the nine-slice background that will display the button graphics
    // A nine-slice allows the button to stretch while preserving corner appearance
    var nineSliceInstance = new NineSliceRuntime
    {
      Height = 0f,
      Texture = atlas.Texture,
      TextureAddress = TextureAddress.Custom
    };
    nineSliceInstance.Dock(Gum.Wireframe.Dock.Fill);
    topLevelContainer.Children.Add(nineSliceInstance);

    // Create the text element that will display the button's label
    var textInstance = new TextRuntime
    {
      // Name is required so it hooks in to the base Button.Text property
      Name = "TextInstance",
      Text = "START",
      Blue = 130,
      Green = 86,
      Red = 70,
      UseCustomFont = true,
      CustomFontFile = "fonts/04b_30.fnt",
      FontScale = 0.25f
    };
    textInstance.Anchor(Gum.Wireframe.Anchor.Center);
    textInstance.Width = 0;
    textInstance.WidthUnits = DimensionUnitType.RelativeToChildren;
    topLevelContainer.Children.Add(textInstance);

    // Get the texture region for the unfocused button state from the atlas
    TextureRegion unfocusedTextureRegion = atlas.GetRegion("unfocused-button");

    // Create an animation chain for the unfocused state with a single frame
    var unfocusedAnimation = new AnimationChain();
    unfocusedAnimation.Name = nameof(unfocusedAnimation);
    var unfocusedFrame = new AnimationFrame
    {
      TopCoordinate = unfocusedTextureRegion.TopTextureCoordinate,
      BottomCoordinate = unfocusedTextureRegion.BottomTextureCoordinate,
      LeftCoordinate = unfocusedTextureRegion.LeftTextureCoordinate,
      RightCoordinate = unfocusedTextureRegion.RightTextureCoordinate,
      FrameLength = 0.3f,
      Texture = unfocusedTextureRegion.Texture
    };
    unfocusedAnimation.Add(unfocusedFrame);

    // Get the multi-frame animation for the focused button state from the atlas
    Animation focusedAtlasAnimation = atlas.GetAnimation("focused-button-animation");

    // Create an animation chain for the focused state using all frames from the atlas animation
    var focusedAnimation = new AnimationChain();
    focusedAnimation.Name = nameof(focusedAnimation);
    foreach (TextureRegion region in focusedAtlasAnimation.Frames)
    {
      var frame = new AnimationFrame
      {
        TopCoordinate = region.TopTextureCoordinate,
        BottomCoordinate = region.BottomTextureCoordinate,
        LeftCoordinate = region.LeftTextureCoordinate,
        RightCoordinate = region.RightTextureCoordinate,
        FrameLength = (float)focusedAtlasAnimation.Delay.TotalSeconds,
        Texture = region.Texture
      };

      focusedAnimation.Add(frame);
    }

    // Assign both animation chains to the nine-slice background
    nineSliceInstance.AnimationChains =
        [
            unfocusedAnimation,
            focusedAnimation
        ];

    // Create a state category for button states
    var category = new StateSaveCategory
    {
      Name = ButtonCategoryName
    };
    topLevelContainer.AddCategory(category);

    // Create the enabled (default/unfocused) state
    var enabledState = new StateSave
    {
      Name = EnabledStateName,
      Apply = () =>
        {
          // When enabled but not focused, use the unfocused animation
          nineSliceInstance.CurrentChainName = unfocusedAnimation.Name;
        }
    };
    category.States.Add(enabledState);

    // Create the focused state
    var focusedState = new StateSave
    {
      Name = FocusedStateName,
      Apply = () =>
        {
          // When focused, use the focused animation and enable animation playback
          nineSliceInstance.CurrentChainName = focusedAnimation.Name;
          nineSliceInstance.Animate = true;
        }
    };
    category.States.Add(focusedState);

    // Create the highlighted+focused state (for mouse hover while focused)
    // by cloning the focused state since they appear the same
    StateSave highlightedFocused = focusedState.Clone();
    highlightedFocused.Name = HighlightedFocusedStateName;
    category.States.Add(highlightedFocused);

    // Create the highlighted state (for mouse hover)
    // by cloning the enabled state since they appear the same
    StateSave highlighted = enabledState.Clone();
    highlighted.Name = HighlightedStateName;
    category.States.Add(highlighted);

    // Add event handlers for keyboard input.
    KeyDown += HandleKeyDown;

    // Add event handler for mouse hover focus.
    topLevelContainer.RollOn += HandleRollOn;

    // Assign the configured container as this button's visual
    Visual = topLevelContainer;
  }

  /// <summary>
  /// Handles keyboard input for navigation between buttons using left/right keys.
  /// </summary>
  private void HandleKeyDown(object sender, KeyEventArgs e)
  {
    if (e.Key == Keys.Left)
    {
      // Left arrow navigates to previous control
      HandleTab(TabDirection.Up, loop: true);
    }
    if (e.Key == Keys.Right)
    {
      // Right arrow navigates to next control
      HandleTab(TabDirection.Down, loop: true);
    }
  }

  /// <summary>
  /// Automatically focuses the button when the mouse hovers over it.
  /// </summary>
  private void HandleRollOn(object sender, EventArgs e)
  {
    IsFocused = true;
  }
}
