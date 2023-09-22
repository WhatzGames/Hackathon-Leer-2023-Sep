'use client';

import {useGameStore} from '@/app/game-store';

export default function Game() {
  const {game} = useGameStore();

  return (
    <div className={'grid grid-cols-3 grid-rows-3 gap-2'}>
      {game.map((board, index) => <Board key={index} boardNo={index} board={board}/>)}
    </div>
  );
}

type BoardProps = {
  boardNo: number;
  board: string[];
}

const Board = ({boardNo, board}: BoardProps) => {
  return (
    <div className={'grid grid-cols-3 grid-rows-3'}>
      {board.map((field, index) => <Field key={index} board={boardNo} field={index}>{field}</Field>)}
    </div>
  );
};

type FieldProps = {
  board: number;
  field: number;
  width?: number;
  height?: number;
}

const Field = ({board, field, width = 60, height = 60, children}: React.PropsWithChildren<FieldProps>) => {
  const setField = useGameStore(state => state.setField);

  const handleClick = () => setField(board, field);

  return (
    <button className={'bg-red-300 border border-black'}
            onClick={handleClick}
            style={{width, height}}>
      {children}
    </button>
  );
};
