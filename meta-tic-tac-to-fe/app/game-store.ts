import {create} from 'zustand';
import {v4 as randomUUID} from 'uuid';

type Game = Board[];
type Board = {
  id: string;
  fields: (Player | null)[];
};
type Player = 'X' | 'O';

type GameState = {
  player: Player;
  activeBoard: number | null;
  finishedBoards: (Player | null)[];
  game: Game;
}

type GameActions = {
  setField: (board: number, field: number) => void;
  switchPlayer: () => void;
  newGame: () => void;
}

const checkBoard = ({fields}: Board): Player | null => {
  const combinations = [
    [0, 1, 2],
    [3, 4, 5],
    [6, 7, 8],
    [0, 3, 6],
    [1, 4, 7],
    [2, 5, 8],
    [0, 4, 8],
    [2, 4, 6],
  ];

  for (const combination of combinations) {
    if (!fields[combination[0]] || !fields[combination[1]] || !fields[combination[2]]) {
      continue;
    }

    if (fields[combination[0]] === fields[combination[1]] && fields[combination[1]] === fields[combination[2]]) {
      return fields[combination[0]];
    }
  }

  return null;
};

const initialState = (): GameState => ({
  player: 'X',
  activeBoard: null,
  finishedBoards: [null, null, null, null, null, null, null, null, null],
  game: [
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
    {id: randomUUID(), fields: [null, null, null, null, null, null, null, null, null]},
  ]
});

export const useGameStore = create<GameState & GameActions>((set, get) => ({
  ...initialState(),
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
    if (game[board].fields[field] != null) {
      return;
    }

    // Set field to player
    game[board].fields[field] = get().player;

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
  },
  switchPlayer() {
    set((state) => ({
      ...state,
      player: state.player === 'X' ? 'O' : 'X',
    }));
  },
  newGame() {
    set((state) => ({
      ...state,
      ...initialState(),
    }));
  }
}));
