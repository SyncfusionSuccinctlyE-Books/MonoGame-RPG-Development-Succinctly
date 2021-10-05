using System;
using System.Collections.Generic;
using MonoGameRPG.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPGEngine;

namespace MonoGameRPG.Screens
{
    public enum CharacterCreationPhase
    {
        LoadNew,
        Stats,
        Race,
        Class,
        Name
    }

    public class CharacterCreationScreen : GameScreen
    {
        private ContentManager content;
        private SpriteFont gameFont;
        private float pauseAlpha;
        private readonly InputAction nameEnteredAction;

        private Vector2 titleVec;

        private Vector2[] statVec;
        private Vector2[] statLabelVec;

        private Rectangle diceButtonRect;
        private Rectangle nextButtonRect;
        private Rectangle backButtonRect;
        private Rectangle cancelButtonRect;
        private Rectangle[] lessButtonRect;
        private Rectangle[] moreButtonRect;
        private Vector2 newCharacterVec;


        private Rectangle[] classButtonRect;
        private Vector2[] classNameVec;
        private Rectangle[] raceButtonRect;
        private Vector2[] raceNameVec;

        private Texture2D rollButton;
        private Texture2D lessButton;
        private Texture2D moreButton;
        private Texture2D saveButton;
        private Texture2D nextButton;
        private Texture2D backButton;
        private Texture2D cancelButton;
        private Texture2D radioButton;
        private Texture2D filledRadioButton;
        private Texture2D newCharacterButton;

        private int[] stats;
        private int remainingPoints = Globals.StatGenPoints;
        private Vector2 statPointsVec;

        private Vector2 charactersTitleVec;

        private Vector2 charactersVec;


        private CharacterCreationPhase curPhase = CharacterCreationPhase.LoadNew;

        private int selectedRaceIndex = -1;
        private int selectedClassIndex = -1;

        private Texture2D nameTextBox;
        private string characterName = "";
        private Rectangle nameRect;
        private Vector2 nameVec;
        private Vector2 nameLabelVec;

        private string[] titles = new string[] { "", "Stats", "Race", "Class", "Name" };

        private string[] characters;

        public CharacterCreationScreen()
        {
            statVec = new Vector2[Globals.Stats.Count];
            statLabelVec = new Vector2[Globals.Stats.Count];

            int count = 0;

            stats = new int[Globals.Stats.Count];

            if (Globals.StatGenType == StatGenerationType.Points)
            {
                for (int i = 0; i < Globals.Stats.Count; i++)
                    stats[i] = Globals.StatStartValue;
            }

            lessButtonRect = new Rectangle[Globals.Stats.Count];
            moreButtonRect = new Rectangle[Globals.Stats.Count];

            for (int i = 0; i < statVec.Length; i++)
            {
                statVec[i] = new Vector2(125, 50 + i * 35);
                statLabelVec[i] = new Vector2(25, 50 + i * 35);
                lessButtonRect[i] = new Rectangle(75, 50 + i * 35, 32, 32);
                moreButtonRect[i] = new Rectangle(175, 50 + i * 35, 32, 32);
            }

            diceButtonRect = new Rectangle(50, 100 + count * 35, 128, 64);
            backButtonRect = new Rectangle(50, 400, 128, 64);

            statPointsVec = new Vector2(300, 75);

            raceButtonRect = new Rectangle[Globals.Races.Count];
            raceNameVec = new Vector2[Globals.Races.Count];

            for (int i = 0; i < Globals.Races.Count; i++)
            {
                raceButtonRect[i] = new Rectangle(25, 50 + i * 35, 32, 32);
                raceNameVec[i] = new Vector2(75, 50 + i * 35);
            }

            classButtonRect = new Rectangle[Globals.Classes.Count];
            classNameVec = new Vector2[Globals.Classes.Count];

            for (int i = 0; i < Globals.Classes.Count; i++)
            {
                classButtonRect[i] = new Rectangle(25, 50 + i * 35, 32, 32);
                classNameVec[i] = new Vector2(75, 50 + i * 35);
            }

            nameRect = new Rectangle(200, 100, 250, 35);
            nameVec = new Vector2(210, 105);
            nameLabelVec = new Vector2(25, 100);

            Keys[] keys = new Keys[28];

            keys[0] = Keys.A;
            keys[1] = Keys.B;
            keys[2] = Keys.C;
            keys[3] = Keys.D;
            keys[4] = Keys.E;
            keys[5] = Keys.F;
            keys[6] = Keys.G;
            keys[7] = Keys.H;
            keys[8] = Keys.I;
            keys[9] = Keys.J;
            keys[10] = Keys.K;
            keys[11] = Keys.L;
            keys[12] = Keys.M;
            keys[13] = Keys.N;
            keys[14] = Keys.O;
            keys[15] = Keys.P;
            keys[16] = Keys.Q;
            keys[17] = Keys.R;
            keys[18] = Keys.S;
            keys[19] = Keys.T;
            keys[20] = Keys.U;
            keys[21] = Keys.V;
            keys[22] = Keys.W;
            keys[23] = Keys.X;
            keys[24] = Keys.Y;
            keys[25] = Keys.Z;
            keys[26] = Keys.Space;
            keys[27] = Keys.Back;

            nameEnteredAction = new InputAction(null, keys, true);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("datafont");

                titleVec = new Vector2(this.ScreenManager.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString(titles[(int)curPhase]).X / 2, 25);

                lessButton = content.Load<Texture2D>(@"Sprites\UI\lessbutton");
                moreButton = content.Load<Texture2D>(@"Sprites\UI\morebutton");
                rollButton = content.Load<Texture2D>(@"Sprites\UI\rolldicebutton");
                nextButton = content.Load<Texture2D>(@"Sprites\UI\nextbutton");
                backButton = content.Load<Texture2D>(@"Sprites\UI\backbutton");
                cancelButton = content.Load<Texture2D>(@"Sprites\UI\cancelbutton");
                saveButton = content.Load<Texture2D>(@"Sprites\UI\savebutton");
                radioButton = content.Load<Texture2D>(@"Sprites\UI\radiobutton");
                filledRadioButton = content.Load<Texture2D>(@"Sprites\UI\filledradiobutton");
                newCharacterButton = content.Load<Texture2D>(@"Sprites\UI\newcharacterbutton");

                nextButtonRect = new Rectangle(this.ScreenManager.GraphicsDevice.Viewport.Width - 178, 400, 128, 64);
                cancelButtonRect = new Rectangle(this.ScreenManager.GraphicsDevice.Viewport.Width / 2 - 64, 400, 128, 64);

                newCharacterVec = new Vector2(this.ScreenManager.GraphicsDevice.Viewport.Width / 2 - 64, this.ScreenManager.GraphicsDevice.Viewport.Height - 75);

                charactersTitleVec = new Vector2(100, 100);
                charactersVec = new Vector2(100, 150);

                nameTextBox = content.Load<Texture2D>(@"Sprites\UI\nametextbox");

                //load existing character names
                characters = GlobalFunctions.GetCharacterNames();

                if (characters != null)
                {
                    for (int i = 0; i < characters.Length; i++)
                        characters[i] = characters[i].Substring(0, characters[i].Length - 4);
                }
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            int playerIndex = (int)ControllingPlayer.Value;
            PlayerIndex index;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];


            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            //if(input.IsNewKeyPress(Keys.Enter, ControllingPlayer.Value, out index))
            //{
            //    ScreenManager.AddScreen(new GameplayScreen(), ControllingPlayer.Value);
            //}

            if (curPhase == CharacterCreationPhase.Name)
            {
                if (nameEnteredAction.Occurred(input, ControllingPlayer, out index))
                {
                    if (keyboardState.GetPressedKeys()[0] == Keys.Back)
                    {
                        characterName = characterName.Substring(0, characterName.Length - 1);
                    }
                    else if (keyboardState.GetPressedKeys()[0] == Keys.Space)
                    {
                        characterName += " ";
                    }
                    else
                    {
                        if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                        {
                            characterName += keyboardState.GetPressedKeys()[0] != Keys.LeftShift && keyboardState.GetPressedKeys()[0] != Keys.RightShift ? keyboardState.GetPressedKeys()[0] : keyboardState.GetPressedKeys()[1];
                        }
                        else
                        {
                            characterName += keyboardState.GetPressedKeys()[0] != Keys.LeftShift && keyboardState.GetPressedKeys()[0] != Keys.RightShift ? keyboardState.GetPressedKeys()[0].ToString().ToLower() : keyboardState.GetPressedKeys()[1].ToString().ToLower();
                        }
                    }
                }
            }

            if (input.IsNewMouseClick(MouseButtons.Left))
            {
                Point point = input.CurrentMouseState[0].Position;

                switch (curPhase)
                {
                    case CharacterCreationPhase.LoadNew:

                        Vector2 vec;
                        int count = 0;
                        Rectangle rect;

                        foreach (string character in characters)
                        {
                            vec = gameFont.MeasureString(character);

                            rect = new Rectangle((int)charactersVec.X, (int)charactersVec.Y + count * 25, (int)vec.X, (int)vec.Y);

                            if (rect.Contains(point))
                            {
                                //set character to be used in game
                                Global.CharacterName = character;
                                ScreenManager.AddScreen(new GameplayScreen(), ControllingPlayer);
                            }
                        }

                        break;

                    case CharacterCreationPhase.Stats:

                        if (Globals.StatGenType == StatGenerationType.Roll)
                        {
                            if (diceButtonRect.Contains(point))
                            {
                                for (int i = 0; i < Globals.Stats.Count; i++)
                                {

                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Globals.Stats.Count; i++)
                            {
                                if (lessButtonRect[i].Contains(point) && stats[i] > Globals.StatStartValue)
                                {
                                    remainingPoints++;
                                    stats[i]--;
                                }
                                else if (moreButtonRect[i].Contains(point) && stats[i] < Stat.MaxValue && remainingPoints > 0)
                                {
                                    remainingPoints--;
                                    stats[i]++;
                                }
                            }
                        }

                        break;

                    case CharacterCreationPhase.Race:

                        for (int i = 0; i < raceButtonRect.Length; i++)
                        {
                            if (raceButtonRect[i].Contains(point))
                            {
                                selectedRaceIndex = i;
                            }
                        }

                        break;

                    case CharacterCreationPhase.Class:

                        for (int i = 0; i < classButtonRect.Length; i++)
                        {
                            if (classButtonRect[i].Contains(point))
                            {
                                selectedClassIndex = i;
                            }
                        }

                        break;

                    case CharacterCreationPhase.Name:

                        if (nextButtonRect.Contains(point))
                        {
                            SaveCharacter();
                        }

                        break;
                }

                if (nextButtonRect.Contains(point))
                {
                    if (curPhase < CharacterCreationPhase.Name)
                        curPhase++;
                    else
                    {
                        SaveCharacter();
                        ScreenManager.AddScreen(new GameplayScreen(), ControllingPlayer);
                    }
                }
                else if (backButtonRect.Contains(point) && curPhase > CharacterCreationPhase.Stats)
                {
                    curPhase--;
                }
                else if (cancelButtonRect.Contains(point))
                {
                    if (curPhase > CharacterCreationPhase.LoadNew)
                    {
                        ExitScreen();
                        ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
                    }
                    else
                        curPhase++;
                }
            }
        }

        private void SaveCharacter()
        {
            Character character = new Character();

            character.Name = characterName;

            for (int i = 0; i < stats.Length; i++)
                character.AddStat(new EntityStat(Globals.Stats[i].Name, (short)stats[i]));

            character.RaceID = Globals.Races[selectedRaceIndex].Name;
            character.ClassID = Globals.Classes[selectedClassIndex].Name;

            GlobalFunctions.SaveCharacter(character);

        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, titles[(int)curPhase], titleVec, Color.White);

            switch (curPhase)
            {
                case CharacterCreationPhase.LoadNew:
                    DrawLoadNewUI(spriteBatch);

                    break;
                case CharacterCreationPhase.Stats:
                    DrawStatsUI(spriteBatch);

                    break;
                case CharacterCreationPhase.Race:
                    DrawRacesUI(spriteBatch);

                    break;
                case CharacterCreationPhase.Class:
                    DrawClassesUI(spriteBatch);

                    break;
                case CharacterCreationPhase.Name:
                    DrawNameUI(spriteBatch);

                    break;
            }

            if (curPhase > CharacterCreationPhase.LoadNew)
            {
                spriteBatch.Draw(nextButton, nextButtonRect, Color.White);
                spriteBatch.Draw(backButton, backButtonRect, Color.White);
                spriteBatch.Draw(cancelButton, cancelButtonRect, Color.White);
            }

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawLoadNewUI(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(gameFont, "Existing Characters", charactersTitleVec, Color.White);
            spriteBatch.Draw(newCharacterButton, newCharacterVec, Color.White);

            int count = 0;

            foreach (string character in characters)
            {
                spriteBatch.DrawString(gameFont, character, Vector2.Add(charactersVec, new Vector2(0, count * 25)), Color.White);
                count++;
            }

        }

        private void DrawStatsUI(SpriteBatch spriteBatch)
        {

            for (int i = 0; i < Globals.Stats.Count; i++)
            {
                spriteBatch.DrawString(gameFont, Globals.Stats[i].Abbreviation + ":", statLabelVec[i], Color.White);
                spriteBatch.DrawString(gameFont, stats[i].ToString(), statVec[i], Color.White);
            }

            if (Globals.StatGenType == StatGenerationType.Points)
            {
                DrawPointsStatsUI(spriteBatch);
            }
            else
            {
                DrawRollStatsUI(spriteBatch);
            }
        }


        private void DrawRacesUI(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Globals.Races.Count; i++)
            {
                spriteBatch.Draw(selectedRaceIndex != i ? radioButton : filledRadioButton, raceButtonRect[i], Color.White);
                spriteBatch.DrawString(gameFont, Globals.Races[i].Name, raceNameVec[i], Color.White);
            }
        }

        private void DrawClassesUI(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Globals.Classes.Count; i++)
            {
                spriteBatch.Draw(selectedClassIndex != i ? radioButton : filledRadioButton, classButtonRect[i], Color.White);
                spriteBatch.DrawString(gameFont, Globals.Classes[i].Name, classNameVec[i], Color.White);
            }
        }

        private void DrawNameUI(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(nameTextBox, nameRect, Color.White);
            spriteBatch.DrawString(gameFont, "Character name:", nameLabelVec, Color.White);
            spriteBatch.DrawString(gameFont, characterName, nameVec, Color.Black);
        }


        private void DrawRollStatsUI(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rollButton, diceButtonRect, Color.White);
        }

        private void DrawPointsStatsUI(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(gameFont, "Remaining Points: " + remainingPoints.ToString(), statPointsVec, Color.White);

            for (int i = 0; i < Globals.Stats.Count; i++)
            {
                spriteBatch.Draw(lessButton, lessButtonRect[i], Color.White);
                spriteBatch.Draw(moreButton, moreButtonRect[i], Color.White);
            }
        }
    }
}