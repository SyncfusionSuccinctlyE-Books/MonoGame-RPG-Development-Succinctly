using Microsoft.Xna.Framework;
using RPGEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MonoGameRPG.Screens
{
    // The main menu screen is the first thing displayed when the game starts up.
    public class MainMenuScreen : MenuScreen
    {

        public MainMenuScreen() : base("Main Menu")
        {
            var playGameMenuEntry = new MenuEntry("Play Game");
            var optionsMenuEntry = new MenuEntry("Options");
            var exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

        }

        public override void Activate(bool instancePreserved)
        {
            GlobalFunctions.LoadClasses();
            GlobalFunctions.LoadRaces();
            GlobalFunctions.LoadStats();

            //Test code for getting json strings for deserializing objects
            //List<EntityGameObject> objs = new List<EntityGameObject>();

            //objs.Add(new EntityGameObject(this.ScreenManager.Game, "Sprites/Test/TestSheet1")
            //{
            //    Entity = new Entity() { Alignment = EntityAlignment.Good, BaseHP = 10, ClassID = "1", Level = 1, Name = "Blacksmith", RaceID = "1", Sex = EntitySex.Male },
            //    Type = ObjectType.Entity,
            //    EntityType = EntityType.NPC,
            //    StartLocation = new Vector2(2, 2),
            //    GameSpriteFileName = "Sprites/Test/TestSheet1"
            //});


            //objs.Add(new EntityGameObject(this.ScreenManager.Game, "Sprites/Test/TestSheet2")
            //{
            //    Entity = new Entity() { Alignment = EntityAlignment.Good, BaseHP = 10, ClassID = "1", Level = 1, Name = "Shopkeeper", RaceID = "1", Sex = EntitySex.Male },
            //    Type = ObjectType.Entity,
            //    EntityType = EntityType.NPC,
            //    StartLocation = new Vector2(6, 6),
            //    GameSpriteFileName = "Sprites/Test/TestSheet2"
            //});


            //string jsonTypeNameAll = JsonConvert.SerializeObject(objs, Formatting.Indented, new JsonSerializerSettings
            //{
            //    TypeNameHandling = TypeNameHandling.All
            //});
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new CharacterCreationScreen());
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";
            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}