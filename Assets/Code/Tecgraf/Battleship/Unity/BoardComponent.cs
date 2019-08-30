using System;
using System.Collections.Generic;
using Tecgraf.Battleship.Core;
using Tecgraf.Battleship.Domain;
using Tecgraf.Battleship.Domain.Ships;
using UnityEngine;
using UnityEngine.UI;

namespace Tecgraf.Battleship.Unity
{
    public class BoardComponent : MonoBehaviour
    {
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private Transform _horizontalLabels;
        [SerializeField] private Transform _verticalLabels;

        [SerializeField] private Dropdown _placementDropdown;
        [SerializeField] private Dropdown _bombardmentDropdown;

        private Player _player;
        private Image[,] _cells;

        public string PlacementStrategy { get { return _placementDropdown.options[_placementDropdown.value].text; } }
        public string BombardmentStrategy { get { return _bombardmentDropdown.options[_bombardmentDropdown.value].text; } }

        public void SetDropdownOptions( List<string> placementOptions, List<string> bombardmentOptions )
        {
            _placementDropdown.AddOptions( placementOptions );
            _bombardmentDropdown.AddOptions( bombardmentOptions );
        }

        public void InitializeGrid( int size, Player p )
        {
            _cells = new Image[size, size];

            _player = p;
            _grid.constraintCount = size;

            for( int i = 0; i < size; i++ )
            {
                CreateLabel( ( (char)( 'A' + i ) ).ToString() ).SetParent( _horizontalLabels );
                CreateLabel( ( i + 1 ).ToString() ).SetParent( _verticalLabels );

                for( int j = 0; j < size; j++ )
                {
                    GameObject cell = Instantiate( _cellPrefab, _grid.transform );
                    _cells[j, i] = cell.GetComponent<Image>();
                }
            }
        }

        private Transform CreateLabel( string label )
        {
            GameObject labelGO = new GameObject( label );
            Text labelText = labelGO.AddComponent<Text>();
            labelText.text = label;
            labelText.fontSize = 20;
            labelText.fontStyle = FontStyle.Bold;
            labelText.color = Color.black;
            labelText.font = Font.CreateDynamicFontFromOSFont( "Arial", 20 );
            labelText.alignment = TextAnchor.MiddleCenter;

            return labelGO.transform;
        }

        public void HideDropdowns()
        {
            _placementDropdown.gameObject.SetActive( false );
            _bombardmentDropdown.gameObject.SetActive( false );
        }

        public void ShowShips()
        {
            for( int i = 0; i < _cells.GetLength( 0 ); i++ )
            {
                for( int j = 0; j < _cells.GetLength( 1 ); j++ )
                {
                    _cells[i, j].color = GetShipColor( _player.PlayerBoard.GetInfo( i, j ) );
                }
            }
        }

        public void MarkBombarded( int x, int y )
        {
            var text = _cells[x, y].GetComponentInChildren<Text>( true );
            text.gameObject.SetActive( true );
        }

        private Color GetShipColor( IBoardEntry entry )
        {
            if( !( entry is IShip ) )
                return Color.white;

            switch( ( entry as IShip ).Name )
            {
                case "Battleship":
                    return Color.yellow;

                case "Carrier":
                    return Color.magenta;

                case "Cruiser":
                    return Color.green;

                case "Destroyer":
                    return Color.cyan;
            }

            return Color.black;
        }
    }
}