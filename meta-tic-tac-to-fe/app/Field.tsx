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
    <button className={'border border-slate-400 rounded-lg text-3xl hover:bg-slate-200 hover:animate-pulse'}
            onClick={handleClick}
            style={{width, height}}>
      {children}
    </button>
  );
};
