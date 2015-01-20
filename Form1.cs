using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers_BY
{
    public partial class Form1 : Form
    {
        private const int indent = 10;
        private const int topIndent = 40;
        private const int playerPictureWidth = 225;
        private const int playerPictureHeight = 300;
        private ChessGame game;

        public Form1()
        {
            InitializeComponent();
            game = new ChessGame();
            game.ChessboardLocation = new Point(indent, topIndent);
            game.PlayerPictureSize = new Size(playerPictureWidth, playerPictureHeight);
            game.ToAddControlsOnForm(this);
            ToRecountSizesAndLocations();
            this.MinimumSize = new Size(3 * indent + playerPictureWidth + playerPictureHeight,
                                        playerPictureHeight + indent + 2 * topIndent);
        }

        private void начатьНовуюИгруToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.ToStartNewGame();
        }
        private void завершитьИгруToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.ToFinishGame();
        }
        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            ToRecountSizesAndLocations();
        }
        private void ToRecountSizesAndLocations()
        {
            game.PlayerPictureLocation = new Point(this.ClientRectangle.Width - indent - playerPictureWidth, topIndent);
            game.ChessboardSize = new Size(this.ClientRectangle.Width - 3 * indent - playerPictureWidth,
                                           this.ClientRectangle.Height - indent - topIndent);
        }
    }

    public class ChessGame // to do /
    {
        private bool gameIsOn;
        private Player whitePlayer;
        private Player blackPlayer;
        private Player activePlayer;
        private Chessboard chessboard;
        private List<ChessFieldPictureBox> fieldPictureBoxes;
        private PictureBox playerPictureBox;
        private Point locationOfUpperLeftCornerOfChessboard;
        private Size sizeOfChessboard;
        private Size sizeOfChessfield;
        private List<IndexesOnBoard> possibleToSelectIndexes;

        public ChessGame()
        {
            gameIsOn = false;
            whitePlayer = new Player(Constants.WhiteColor);
            blackPlayer = new Player(Constants.BlackColor);
            chessboard = new Chessboard();
            playerPictureBox = new PictureBox();
            playerPictureBox.BackgroundImageLayout = ImageLayout.Zoom;
            fieldPictureBoxes = new List<ChessFieldPictureBox>();
            var k = 0;
            for (var i = 0; i < Constants.ChessboardDimension; i++)
                for (var j = 0; j < Constants.ChessboardDimension; j++)
                {
                    fieldPictureBoxes.Add(new ChessFieldPictureBox());
                    fieldPictureBoxes[k].Indexes.Row = i;
                    fieldPictureBoxes[k].Indexes.Column = j;
                    fieldPictureBoxes[k].Enabled = false;
                    fieldPictureBoxes[k].BackgroundImageLayout = ImageLayout.Zoom;
                    fieldPictureBoxes[k].Click += new EventHandler(ToProcessClickOnFieldPictureBox);
                    k++;
                }
        }

        public Point PlayerPictureLocation
        {
            get
            {
                return playerPictureBox.Location;
            }
            set
            {
                playerPictureBox.Location = value;
            }
        }
        public Size PlayerPictureSize
        {
            get
            {
                return playerPictureBox.Size;
            }
            set
            {
                playerPictureBox.Size = value;
            }
        }        
        public Point ChessboardLocation
        {
            get
            {
                return locationOfUpperLeftCornerOfChessboard;
            }
            set
            {
                locationOfUpperLeftCornerOfChessboard = value;
                var k = 0;
                for (var i = 0; i < Constants.ChessboardDimension; i++)
                    for (var j = 0; j < Constants.ChessboardDimension; j++)
                    {
                        fieldPictureBoxes[k].Location = new Point(locationOfUpperLeftCornerOfChessboard.X + sizeOfChessfield.Width * j,
                                                                  locationOfUpperLeftCornerOfChessboard.Y
                                                                  + sizeOfChessfield.Height * (Constants.ChessboardDimension - i - 1)); 
                        k++;
                    }
            }
        }
        public Size ChessboardSize
        {
            get
            {
                return sizeOfChessboard;
            }
            set
            {
                sizeOfChessboard = value;
                var sideLength = Math.Min(sizeOfChessboard.Width, sizeOfChessboard.Height);
                sideLength /= Constants.ChessboardDimension;
                sizeOfChessfield = new Size(sideLength, sideLength);
                foreach (var fieldPictureBox in fieldPictureBoxes)
                {
                    fieldPictureBox.Size = sizeOfChessfield;
                }
                ChessboardLocation = locationOfUpperLeftCornerOfChessboard;
            }
        }

        public void ToAddControlsOnForm(Form form)
        {
            form.Controls.Add(playerPictureBox);
            for (var i = 0; i < fieldPictureBoxes.Count; i++)
            {
                form.Controls.Add(fieldPictureBoxes[i]);
            }
        }
        public void ToStartNewGame()
        {
            bool itIsNecessaryToStartGame = true;
            if (gameIsOn)
            {
                var response = MessageBox.Show("Текущая игра не завершена и будет утеряна. Начать новую игру?",
                                               "Новая игра", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (response == DialogResult.No)
                {
                    itIsNecessaryToStartGame = false;
                }
            }
            if (itIsNecessaryToStartGame)
            {
                chessboard.ToCleanOfFigures();
                ToDistributeAndPlaceFiguresToPlayer(whitePlayer);
                chessboard.ToTurnBoardOn180();
                ToDistributeAndPlaceFiguresToPlayer(blackPlayer);
                chessboard.ToTurnBoardOn180();
                activePlayer = whitePlayer;
                activePlayer.UpdateLists();
                possibleToSelectIndexes = activePlayer.CoursesSourceIndexes;
                foreach (var field in fieldPictureBoxes)
                {
                    field.Enabled = true;
                }
                gameIsOn = true;
                ToUpdatePictures();
                ToMakeSelections();
            }
        }
        public void ToFinishGame()
        {
            bool itIsNecessaryToFinishGame = gameIsOn;
            if (gameIsOn)
            {
                var response = MessageBox.Show("Текущая игра не завершена и будет утеряна. Завершить игру?",
                                               "Завершение игры", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (response == DialogResult.No)
                {
                    itIsNecessaryToFinishGame = false;
                }
            }
            if (itIsNecessaryToFinishGame)
            {
                gameIsOn = false;
                playerPictureBox.BackgroundImage = null;
                foreach (var field in fieldPictureBoxes)
                {
                    field.Enabled = false;
                    field.BackColor = Color.Transparent;
                    field.BackgroundImage = null;
                    field.Image = null;
                }
            }
        }
        private void ToDistributeAndPlaceFiguresToPlayer(Player player)
        {
            IndexesOnBoard indexes;
            player.Figures = new List<ChessFigure>();
            var k = 0;
            for (var i = 0; i < 3; i++)
            {
                indexes.Row = i;
                for (var j = 0; j < Constants.ChessboardDimension; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        indexes.Column = j;
                        player.Figures.Add(new Checker(player.PlayerColor));
                        player.Figures[k].TryPutInField(chessboard, indexes);
                        k++;
                    }
                }
            }
        }
        private void ToUpdatePictures()
        {
            if (gameIsOn)
            {
                playerPictureBox.BackgroundImage = activePlayer.PlayerImage;
                var k = 0;
                for (var i = 0; i < Constants.ChessboardDimension; i++)
                    for (var j = 0; j < Constants.ChessboardDimension; j++)
                    {
                        switch (chessboard[i, j].FieldColor)
                        {
                            case Constants.WhiteColor: fieldPictureBoxes[k].BackColor = Color.FromArgb(246, 244, 203); break;
                            case Constants.BlackColor: fieldPictureBoxes[k].BackColor = Color.FromArgb(114, 86, 58); break;
                            default: fieldPictureBoxes[k].BackColor = Color.Transparent; break;
                        }
                        if (chessboard[i, j].WhoIsInField() != null)
                            fieldPictureBoxes[k].BackgroundImage = chessboard[i, j].WhoIsInField().FigureImage;
                        else
                            fieldPictureBoxes[k].BackgroundImage = null;
                        fieldPictureBoxes[k].Image = null;
                        k++;
                    }
            }
        }
        private void ToMakeSelections()
        {
            foreach (var field in fieldPictureBoxes)
            {
                if (possibleToSelectIndexes.Contains(field.Indexes))
                    field.Image = Properties.Resources.selection_red;
                else
                    field.Image = null;
            }
        }
        private void ToProcessClickOnFieldPictureBox(object sender, EventArgs e) //to do /
        {
            var fieldPictureBox = sender as ChessFieldPictureBox;
            var indexes = fieldPictureBox.Indexes;
            var row = indexes.Row;
            var column = indexes.Column;
            if (gameIsOn)
            {
                if (possibleToSelectIndexes.Contains(indexes))
                {
                    if (activePlayer.FigureInHand == null)
                    {
                        activePlayer.TryToTakeFigureInHand(chessboard, indexes);
                        switch (activePlayer.WhatDoesPlayerDo)
                        {
                            case Constants.PlayerDoesCourse: 
                                possibleToSelectIndexes = activePlayer.FigureInHand.CoursesDestinationIndexes; break;
                            case Constants.PlayerDoesCapture: 
                                possibleToSelectIndexes = activePlayer.FigureInHand.CapturesDestinationIndexes; break;
                        }
                    }
                    else
                    {
                        activePlayer.TryToMoveFigureInHandToField(chessboard, indexes);
                        switch (activePlayer.WhatDoesPlayerDo)
                        {
                            case Constants.PlayerDoesCourse:
                            {
                                if (activePlayer == whitePlayer)
                                    activePlayer = blackPlayer;
                                else
                                    activePlayer = whitePlayer;
                                chessboard.ToTurnBoardOn180();
                                activePlayer.UpdateLists();
                                if (activePlayer.CapturesSourceIndexes.Count != 0)
                                    possibleToSelectIndexes = activePlayer.CapturesSourceIndexes;
                                else if (activePlayer.CoursesSourceIndexes.Count != 0)
                                    possibleToSelectIndexes = activePlayer.CoursesSourceIndexes;
                                else
                                {
                                    possibleToSelectIndexes = new List<IndexesOnBoard>();
                                    foreach (var field in fieldPictureBoxes)
                                    {
                                        field.Enabled = false;
                                    }
                                    ToUpdatePictures();
                                    gameIsOn = false;
                                    switch (activePlayer.PlayerColor)
                                    {
                                        case Constants.WhiteColor: MessageBox.Show("Игра окончена. Белый игрок проиграл"); break;
                                        case Constants.BlackColor: MessageBox.Show("Игра окончена. Черный игрок проиграл"); break;
                                    }
                                }
                                break;
                            }
                            case Constants.PlayerDoesCapture:
                            {
                                //снять с поля шашку, которую побили, если игрок бьёт
                                /*activePlayer.UpdateLists();
                                activePlayer.TryToTakeFigureInHand(chessboard, indexes);
                                switch (activePlayer.WhatDoesPlayerDo)
                                {
                                    case Constants.PlayerDoesCourse: 
                                        possibleToSelectIndexes = activePlayer.FigureInHand.CoursesDestinationIndexes; break;
                                    case Constants.PlayerDoesCapture: 
                                        possibleToSelectIndexes = activePlayer.FigureInHand.CapturesDestinationIndexes; break;
                                }*/
                                break;
                            }
                        }
                        ToUpdatePictures();
                    }
                    ToMakeSelections();
                }
            }
        }
    }

    public class Player
    {
        public Image PlayerImage;
        public List<ChessFigure> Figures;
        private bool colorIsSet;
        private byte playerColor;
        private byte whatDoesPlayerDo;
        private List<IndexesOnBoard> fromWhereItIsPossibleToGo;
        private List<IndexesOnBoard> fromWhereItIsPossibleToBeat;
        private ChessFigure figureInHand;

        public Player()
        {
            colorIsSet = false;
        }
        public Player(byte newPlayerColor)
        {
            PlayerColor = newPlayerColor;
        }

        public byte PlayerColor
        {
            get
            {
                if (colorIsSet)
                    return playerColor;
                else
                    return Constants.UndefinedColor;
            }
            set
            {
                if (colorIsSet == false)
                {
                    playerColor = value;
                    colorIsSet = true;
                    switch (playerColor)
                    {
                        case Constants.WhiteColor: PlayerImage = Properties.Resources.player_white; break;
                        case Constants.BlackColor: PlayerImage = Properties.Resources.player_black; break;
                    }
                }
            }
        }
        public byte WhatDoesPlayerDo
        {
            get
            {
                return whatDoesPlayerDo;
            }
        }
        public List<IndexesOnBoard> CoursesSourceIndexes
        {
            get
            {
                return fromWhereItIsPossibleToGo;
            }
        }
        public List<IndexesOnBoard> CapturesSourceIndexes
        {
            get
            {
                return fromWhereItIsPossibleToBeat;
            }
        }
        public ChessFigure FigureInHand
        {
            get
            {
                return figureInHand;
            }
        }

        public void UpdateLists()
        {
            foreach (var figure in Figures)
            {
                if (figure.WhereIsFigure() == null)
                    Figures.Remove(figure);
            }
            fromWhereItIsPossibleToGo = new List<IndexesOnBoard>();
            fromWhereItIsPossibleToBeat = new List<IndexesOnBoard>();
            foreach (var figure in Figures)
            {
                figure.UpdateLists();
                var figureIndexes = figure.GetIndexesForBoardArray();
                if (figure.CoursesDestinationIndexes.Count != 0)
                    fromWhereItIsPossibleToGo.Add(figureIndexes);
                if (figure.CapturesDestinationIndexes.Count != 0)
                    fromWhereItIsPossibleToBeat.Add(figureIndexes);
            }
            if (fromWhereItIsPossibleToGo.Count == 0 && fromWhereItIsPossibleToBeat.Count == 0)
                whatDoesPlayerDo = Constants.PlayerLost;
            else
                whatDoesPlayerDo = Constants.PlayerWaits;
        }    
        public bool TryToTakeFigureInHand(Chessboard board, IndexesOnBoard takeIndexes)
        {
            bool took = false;
            if (board != null)
            {
                figureInHand = board[takeIndexes.Row, takeIndexes.Column].WhoIsInField();
                if (figureInHand != null)
                {
                    if (fromWhereItIsPossibleToBeat.Contains(takeIndexes))
                    {
                        whatDoesPlayerDo = Constants.PlayerDoesCapture;
                        took = true;
                    }
                    else if (fromWhereItIsPossibleToGo.Contains(takeIndexes))
                    {
                        whatDoesPlayerDo = Constants.PlayerDoesCourse;
                        took = true;
                    }
                }
            }
            return took;
        }
        public bool TryToMoveFigureInHandToField(Chessboard board, IndexesOnBoard destinationIndexes)
        {
            bool moved = false;
            if (figureInHand.WhereIsFigure() != board[destinationIndexes.Row, destinationIndexes.Column])
            {
                if (board[destinationIndexes.Row, destinationIndexes.Column].WhoIsInField() != null)
                    board[destinationIndexes.Row, destinationIndexes.Column].WhoIsInField().TryRemoveFromField();
                if (figureInHand.TryRemoveFromField())
                    if (figureInHand.TryPutInField(board, destinationIndexes))
                    {
                        moved = true;
                        figureInHand = null;
                    }                           
            }
            return moved;
        }
    }

    public class Chessboard
    {
        private ChessField[,] fields;
        private bool isTurned;

        public Chessboard()
        {
            ToCleanOfFigures();
        }

        public ChessField this[int row, int column]
        {
            get
            {
                return fields[row, column];
            }
        }
        public bool IsTurned
        {
            get
            {
                return isTurned;
            }
        }

        public void ToCleanOfFigures()
        {
            isTurned = false;
            fields = new ChessField[Constants.ChessboardDimension, Constants.ChessboardDimension];
            for (var i = 0; i < Constants.ChessboardDimension; i++)
                for (var j = 0; j < Constants.ChessboardDimension; j++)
                    if ((i + j) % 2 == 0)
                        fields[i, j] = new ChessField(Constants.BlackColor);
                    else
                        fields[i, j] = new ChessField(Constants.WhiteColor);
        }
        public void ToTurnBoardOn180()
        {
            for (var i = 0; i < Constants.ChessboardDimension / 2; i++)
                for (var j = 0; j < Constants.ChessboardDimension; j++)
                {
                    var tempField = fields[i, j];
                    fields[i, j] = fields[Constants.ChessboardDimension - i - 1, Constants.ChessboardDimension - j - 1];
                    fields[Constants.ChessboardDimension - i - 1, Constants.ChessboardDimension - j - 1] = tempField;
                }
            isTurned = !isTurned;
        }
        public IndexesOnBoard TransformIndexesIfIsTurned(IndexesOnBoard staticIndexes)
        {
            IndexesOnBoard transformedIndexes;
            if (isTurned)
            {
                transformedIndexes.Row = Constants.ChessboardDimension - staticIndexes.Row - 1;
                transformedIndexes.Column = Constants.ChessboardDimension - staticIndexes.Column - 1;
            }
            else
            {
                transformedIndexes = staticIndexes;
            }
            return staticIndexes;
        }
    }

    public class ChessField
    {
        private bool colorIsSet;
        private byte fieldColor;
        private ChessFigure figure;

        public ChessField()
        {
            colorIsSet = false;
        }
        public ChessField(byte newFieldColor)
        {
            FieldColor = newFieldColor;
        }

        public byte FieldColor
        {
            get
            {
                if (colorIsSet)
                    return fieldColor;
                else
                    return Constants.UndefinedColor;
            }
            set
            {
                if (colorIsSet == false)
                {
                    fieldColor = value;
                    colorIsSet = true;
                }
            }
        }

        public ChessFigure WhoIsInField()
        {          
            return figure;
        }
        public bool TryPutFigure(ChessFigure putFigure)
        {
            bool figureIsPut = false;
            if (figure == null)
            {
                if (this == putFigure.WhereIsFigure())
                {
                    figure = putFigure;
                    figureIsPut = true;
                }
            }
            return figureIsPut;
        }
        public ChessFigure TryRemoveFigure()
        {
            ChessFigure removedFigure = null;
            if (figure != null)
            {
                if (this == figure.WhereIsFigure())
                {
                    removedFigure = figure;
                    figure = null;
                }
            }
            return removedFigure;
        }
    }

    public abstract class ChessFigure
    {
        public Image FigureImage;
        private bool colorIsSet;
        private byte figureColor;
        private Chessboard chessboard;
        private IndexesOnBoard staticIndexes;
        private List<IndexesOnBoard> whereItIsPossibleToGo;
        private List<IndexesOnBoard> whereItIsPossibleToBeat;

        public ChessFigure()
        {
            colorIsSet = false;
            staticIndexes.Column = staticIndexes.Row = Constants.UndefinedIndex;
        }

        public byte FigureColor
        {
            get
            {
                if (colorIsSet)
                    return figureColor;
                else
                    return Constants.UndefinedColor;
            }
            set
            {
                if (colorIsSet == false)
                {
                    figureColor = value;
                    colorIsSet = true;
                }
            }
        }
        public List<IndexesOnBoard> CoursesDestinationIndexes
        {
            get
            {
                return whereItIsPossibleToGo;
            }
        }
        public List<IndexesOnBoard> CapturesDestinationIndexes
        {
            get
            {
                return whereItIsPossibleToBeat;
            }
        }
        public Chessboard Board
        {
            get
            {
                return chessboard;
            }
        }

        public bool TryPutInField(Chessboard onBoard, IndexesOnBoard setStaticIndexes)
        {
            bool figureIsPut = false;
            if (WhereIsFigure() == null)
            {
                chessboard = onBoard;
                staticIndexes = chessboard.TransformIndexesIfIsTurned(setStaticIndexes);
                if (WhereIsFigure().TryPutFigure(this))
                {
                    figureIsPut = true;
                }
                else
                {
                    chessboard = null;
                    staticIndexes.Row = staticIndexes.Column = Constants.UndefinedIndex;
                }
            }
            return figureIsPut;
        }
        public bool TryRemoveFromField()
        {
            bool figureIsRemoved = false;
            if (WhereIsFigure() != null)
            {
                if (this == WhereIsFigure().WhoIsInField())
                {
                    WhereIsFigure().TryRemoveFigure();
                    chessboard = null;
                    staticIndexes.Row = staticIndexes.Column = Constants.UndefinedIndex;
                    figureIsRemoved = true;
                }
            }
            return figureIsRemoved;
        }
        public ChessField WhereIsFigure()
        {
            ChessField figureIsHere = null;
            IndexesOnBoard indexesForArray;
            if (chessboard != null)
            {
                indexesForArray = GetIndexesForBoardArray();
                figureIsHere = chessboard[indexesForArray.Row, indexesForArray.Column];
            }
            return figureIsHere;
        }
        public IndexesOnBoard GetIndexesForBoardArray()
        {
            IndexesOnBoard result;
            if (chessboard != null)
            {
                result = chessboard.TransformIndexesIfIsTurned(staticIndexes);
            }
            else
            {
                result.Row = Constants.UndefinedIndex;
                result.Column = Constants.UndefinedIndex;
            }
            return result;
        }
        public void UpdateLists()
        {
            whereItIsPossibleToGo = ToMakeListOfAvailableCourses();
            whereItIsPossibleToBeat = ToMakeListOfPossibleCaptures();
        }
        public abstract List<IndexesOnBoard> ToMakeListOfAvailableCourses();
        public abstract List<IndexesOnBoard> ToMakeListOfPossibleCaptures();
    }

    public class ChessFieldPictureBox : PictureBox
    {
        public IndexesOnBoard Indexes;
    }

    public class Checker : ChessFigure // to do later/
    {
        public Checker(byte newFigureColor)
        {
            FigureColor = newFigureColor;
            switch (FigureColor)
            {
                case Constants.WhiteColor: FigureImage = Properties.Resources.checker_white; break;
                case Constants.BlackColor: FigureImage = Properties.Resources.checker_black; break;
            }
        }

        public override List<IndexesOnBoard> ToMakeListOfAvailableCourses()
        {
            var whereItIsPossibleToGo = new List<IndexesOnBoard>();
            IndexesOnBoard possibleIndexes;
            var figureIndexes = this.GetIndexesForBoardArray();
            var upperRow = figureIndexes.Row + 1;
            var leftColumn = figureIndexes.Column - 1;
            var rightColumn = figureIndexes.Column + 1;
            if (upperRow < Constants.ChessboardDimension)
            {
                if (leftColumn >= 0)
                    if (this.Board[upperRow, leftColumn].WhoIsInField() == null)
                    {
                        possibleIndexes.Row = upperRow;
                        possibleIndexes.Column = leftColumn;
                        whereItIsPossibleToGo.Add(possibleIndexes);
                    }
                if (rightColumn < Constants.ChessboardDimension)
                    if (this.Board[upperRow, rightColumn].WhoIsInField() == null)
                    {
                        possibleIndexes.Row = upperRow;
                        possibleIndexes.Column = rightColumn;
                        whereItIsPossibleToGo.Add(possibleIndexes);
                    }
            }          
            return whereItIsPossibleToGo;
        }
        public override List<IndexesOnBoard> ToMakeListOfPossibleCaptures() // to do /
        {
            var whereItIsPossibleToBeat = new List<IndexesOnBoard>();

            return whereItIsPossibleToBeat;
        }
    }

    public struct IndexesOnBoard
    {
        public int Row;
        public int Column;
    }

    public static class Constants
    {
        public const int ChessboardDimension = 8;
        public const int UndefinedIndex = -1;
        public const byte UndefinedColor = 0;
        public const byte WhiteColor = 1;
        public const byte BlackColor = 2;
        public const byte PlayerWaits = 0;
        public const byte PlayerDoesCourse = 1;
        public const byte PlayerDoesCapture = 2;
        public const byte PlayerLost = 3;
    }
}
