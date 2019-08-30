using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;

namespace Tecgraf.Battleship.Unity.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;

        [SerializeField] private Transform _gameArea;
        [SerializeField] private InputField _simulationCount;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Selectable[] _selectables;
        [SerializeField] private Text _statusBar;

        [SerializeField] private GameObject _playerBoardPrefab;


        public List<BoardComponent> PlayerBoards { get; set; }
        public int SimulationCount { get { return int.Parse( _simulationCount.text ); } set { _simulationCount.text = string.Format( "{0}", value ); } }

        public void Clear()
        {
            foreach( Transform t in _gameArea )
                Destroy( t.gameObject );

            PlayerBoards = new List<BoardComponent>();
        }

        public BoardComponent AddBoard()
        {
            GameObject boardGO = Instantiate( _playerBoardPrefab, _gameArea );
            var bc = boardGO.GetComponent<BoardComponent>();
            PlayerBoards.Add( bc );

            return bc;
        }

        public void UpdateStatus( Player bombardier, Player bombarded, int x, int y, IBoardEntry hit )
        {
            PlayerBoards[bombarded.ID].MarkBombarded( x, y );

            bool success = !( hit is EmptyBoardCell );

            _statusBar.text = string.Format
            (
                "<i>{0}</i> BOMBARDED <i>{1}</i> at <b>{2}{3}</b>. <color={4}>{5}</color>.",
                bombardier.Name,
                bombarded.Name,
                (char)( 'A' + x ),
                y + 1,
                success ? "red" : "blue",
                success ? "HIT" : "MISS"
            );

            if( hit is IShip )
                _statusBar.text += string.Format( " {0}'s {1} has been destroyed!", bombarded.Name, ( hit as IShip ).Name );

            if( !bombarded.IsAlive )
                _statusBar.text += string.Format( " <b>GAME OVER!</b> <color=green>{0}</color> won after {1} attempts!", bombardier.Name, bombardier.Attempts );
        }

        public void SetStatusText(string text)
        {
            _statusBar.text = text;
        }

        public void ResetButtons()
        {
            _newGameButton.interactable = true;

            foreach( var s in _selectables )
                s.interactable = false;
        }


        #region UI Events
        public void OnNewGameButtonPressed()
        {
            _gameController.NewGame();
        }

        public void OnPlaceShipsButtonPressed()
        {
            _gameController.PlaceShips();
        }

        public void OnSimulateButtonPressed()
        {
            StartCoroutine( _gameController.Simulate() );
        }

        public void OnStopButtonPressed()
        {
            StopCoroutine( _gameController.Simulate() );
        }

        public void OnBackgroundSimulateButtonPressed()
        {
            StartCoroutine( _gameController.BackgroundSimulate() );
        }

        public void OnPlayer1BombardButtonPressed()
        {
            _gameController.Bombard( _gameController.Players[1], _gameController.Players[0] );
        }

        public void OnPlayer2BombardButtonPressed()
        {
            _gameController.Bombard( _gameController.Players[0], _gameController.Players[1] );
        }

        public void OnPlayer1SimulateTogglePressed( bool value )
        {
            _gameController.SimulatePlayers[0] = value;
        }

        public void OnPlayer2SimulateTogglePressed( bool value )
        {
            _gameController.SimulatePlayers[1] = value;
        }
        #endregion
    }
}