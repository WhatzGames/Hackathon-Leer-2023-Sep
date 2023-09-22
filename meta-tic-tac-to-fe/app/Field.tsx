import {useGameStore} from '@/app/game-store';

export type FieldProps = {
  board: number;
  field: number;
  width?: number;
  height?: number;
}

export default function Field({board, field, width = 80, height = 80, children}: React.PropsWithChildren<FieldProps>) {
  const setField = useGameStore(state => state.setField);

  const handleClick = () => setField(board, field);

  return (
    <button className={'border border-cyan-400 text-3xl hover:bg-cyan-300 hover:animate-pulse'}
            onClick={handleClick}
            style={{width, height}}>
      {children}
    </button>
  );
};
