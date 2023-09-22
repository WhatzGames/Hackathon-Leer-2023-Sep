import {create} from 'zustand';

type Game = Board[];
type Board = (Player | null)[];
type Player = 'X' | 'O';

type GameStore = {
  player: Player;
  activeBoard: number | null;
  finishedBoards: (Player | null)[];
  game: Game;
  setField: (board: number, field: number) => void;
}

const checkBoard = (board: Board): Player | null => {
  const combinations = [
    [0,1,2],
    [3,4,5],
    [6,7,8],
    [0,3,6],
    [1,4,7],
    [2,5,8],
    [0,4,8],
    [2,4,6],
  ];

  for (const combination of combinations) {
    if (!board[combination[0]] || !board[combination[1]] || !board[combination[2]]) {
      continue;
    }

    if (board[combination[0]] === board[combination[1]] && board[combination[1]] === board[combination[2]]) {
      return board[combination[0]];
    }
  }

  return null;
}

export const useGameStore = create<GameStore>((set, get) => ({
  player: 'X',
  activeBoard: null,
  finishedBoards: [null, null, null, null, null, null, null, null, null],
  game: [
    [null, null, null, null, null, null, null, null, null],
    [null, null, null, null, null, null, null, null, null],
    [null, null, null, null, null, null, null, null, null],
    [null, null, null, null, null, null, null, null, null],
    [null, null, null, null, null, null, null, null, null],
    [null, null, null, null, null, null, null, null, null],
    [null, null, null, null, null, null, null, null, null],
    [null, null, null, null, null, null, null, null, null],
    [null, null, null, null, null, null, null, null, null],
  ],
  setField(board: number, field: number) {
    // board not active - not allowed
    let activeBoard = get().activeBoard;
    if (activeBoard !== null && board !== activeBoard) {
      return;
    }

    // Board finished - not allowed
    const finishedBoards = get().finishedBoards;
    if (finishedBoards[board] !== null) {
      return;
    }

    // Field set - not allowed
    const game = get().game;
    if (game[board][field] != null) {
      return;
    }

    // Set field to player
    game[board][field] = get().player;

    const winner = checkBoard(game[board]);
    if (winner) {
      finishedBoards[board] = winner;
    }

    // Set new active to last field, otherwise allow random choice
    activeBoard = finishedBoards[field] != null ? null : field;

    set((state) => ({
      ...state,
      player: state.player === 'X' ? 'O' : 'X',
      activeBoard,
      finishedBoards,
      game
    }));
  }
}));
