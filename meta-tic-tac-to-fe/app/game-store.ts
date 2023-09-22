import {create} from 'zustand';

type Game = string[][];
type Player = 'X' | 'O';

type GameStore = {
  player: Player;
  game: Game;
  setField: (board: number, field: number) => void;
}

const setField = (game: Game, player: Player, board: number, field: number): Game => {
  game[board][field] = player;
  return game;
};

export const useGameStore = create<GameStore>((set, get) => ({
  player: 'X',
  game: [
    ['', '', '', '', '', '', '', '', ''],
    ['', '', '', '', '', '', '', '', ''],
    ['', '', '', '', '', '', '', '', ''],
    ['', '', '', '', '', '', '', '', ''],
    ['', '', '', '', '', '', '', '', ''],
    ['', '', '', '', '', '', '', '', ''],
    ['', '', '', '', '', '', '', '', ''],
    ['', '', '', '', '', '', '', '', ''],
    ['', '', '', '', '', '', '', '', ''],
  ],
  setField(board: number, field: number) {
    const game = get().game;

    if (game[board][field] !== '') {
      return; // value set - not allowed!
    }

    set((state) => ({
      ...state,
      player: state.player === 'X' ? 'O' : 'X',
      game: setField(state.game, state.player, board, field)
    }));
  }
}));
