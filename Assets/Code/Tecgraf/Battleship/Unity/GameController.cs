using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

using Tecgraf.Battleship.Strategies;
using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain.Ships;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Unity.UI;

namespace Tecgraf.Battleship.Unity
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameUI _gameUI;

        [SerializeField] private int _gridSize;


        public List<Player> Players { get; set; }
        public bool[] SimulatePlayers { get; set; } = new bool[] { true, true };

        private bool _gameOver;

        private void Start()
        {
            NewGame();
        }

        public void NewGame()
        {
            _gameUI.Clear();

            _gameOver = false;

            Players = new List<Player>();

            var placements = GetStrategies<IPlacementStrategy>();
            var bombardments = GetStrategies<IBombardmentStrategy>();

            for( int i = 0; i < 2; i++ )
            {
                _gameUI.AddBoard();
                _gameUI.PlayerBoards[i].SetDropdownOptions( placements, bombardments );

                Players.Add(new Player(_gridSize)
                {
                    Name = string.Format("Player {0}", i + 1),
                    ID = i,
                    Ships = new List<IShip>()
                    {

                        new ShipCarrier(),
                        new ShipBattleship(),
                        new ShipBattleship(),
                        new ShipCruiser(),
                        new ShipCruiser(),
                        new ShipCruiser(),
                        new ShipDestroyer(),
                        new ShipDestroyer(),
                        new ShipDestroyer(),
                        new ShipDestroyer(),
                    },
                });
            }
        }

        public void PlaceShips()
        {
            for( int i = 0; i < Players.Count; i++ )
            {
                Players[i].PlacementStrategy = InstantiateStrategy<IPlacementStrategy>( _gameUI.PlayerBoards[i].PlacementStrategy );
                Players[i].BombardmentStrategy = InstantiateStrategy<IBombardmentStrategy>( _gameUI.PlayerBoards[i].BombardmentStrategy );

                _gameUI.PlayerBoards[i].HideDropdowns();
                _gameUI.PlayerBoards[i].InitializeGrid( _gridSize, Players[i] );

                Players[i].PlaceShips();
                _gameUI.PlayerBoards[i].ShowShips();
            }
        }
        
        public IEnumerator BackgroundSimulate()
        {
            int numAttempts = _gameUI.SimulationCount;
            float avgAttempts = 0;

            string bombardmentStrategy = _gameUI.PlayerBoards[0].BombardmentStrategy;
            string placementStrategy = _gameUI.PlayerBoards[1].PlacementStrategy;

            for( int i = 0; i < numAttempts; i++ )
            {
                NewGame();

                Players[0].BombardmentStrategy = InstantiateStrategy<IBombardmentStrategy>( bombardmentStrategy );
                Players[1].PlacementStrategy = InstantiateStrategy<IPlacementStrategy>( placementStrategy );
                Players[1].PlaceShips();

                while( !_gameOver )
                {
                    if( Players[1].IsAlive )
                        Bombard( Players[0], Players[1], true );

                    if( Players[0].Attempts > ( _gridSize * _gridSize ) )
                    {
                        Debug.LogError( "Something went wrong!" );
                        _gameOver = true;
                    }
                }

                //_newGameButton.interactable = false;

                avgAttempts += Players[0].Attempts;
                _gameUI.SimulationCount = numAttempts - i - 1;
                _gameUI.SetStatusText( string.Format( "Simulating: {0:P0}", i / (float)numAttempts ) );

                yield return new WaitForEndOfFrame();
            }

            avgAttempts /= numAttempts;

            _gameUI.SetStatusText( string.Format( "Average victory for <color='red'>{0}</color> x <color='blue'>{1}</color> = {2} attempts", 
                bombardmentStrategy, 
                placementStrategy, 
                avgAttempts ) );

            //_newGameButton.interactable = true;
        }

        public IEnumerator Simulate()
        {
            int index = 0;
            while( !_gameOver )
            {
                if( SimulatePlayers[index] )
                {
                    if( Players[( index + 1 ) % 2].IsAlive )
                        Bombard( Players[index], Players[( index + 1 ) % 2] );
                }

                yield return new WaitForEndOfFrame();

                index = ( index + 1 ) % 2;
            }
        }

        private void GameOver()
        {
            _gameOver = true;

            _gameUI.ResetButtons();

            StopCoroutine( Simulate() );
        }

        public void Bombard( Player bombardier, Player bombarded, bool silent = false )
        {
            bombardier.Bombard( bombarded, out int x, out int y, out IBoardEntry hit );

            if( !bombarded.IsAlive )
                GameOver();

            if( !silent )
                _gameUI.UpdateStatus( bombardier, bombarded, x, y, hit );
        }

        private List<string> GetStrategies<T>()
        {
            return typeof( T ).Assembly.GetTypes()
                .Where( p => p.IsClass && typeof( T ).IsAssignableFrom( p ) )
                .Select( t => GetTypeName( t ) )
                .ToList();
        }

        private T InstantiateStrategy<T>( string strategy )
        {
            var type = typeof(T).Assembly.GetTypes()
                .FirstOrDefault( p => p.IsClass && typeof(T).IsAssignableFrom( p ) && GetTypeName( p ) == strategy );

            return (T)Activator.CreateInstance( type );
        }

        private string GetTypeName( Type type )
        {
            return string.Join( " ", Regex.Matches( type.Name, @"([A-Z][a-z]+)" ).Cast<System.Text.RegularExpressions.Match>().Select( m => m.Value ) );
        }
    }
}