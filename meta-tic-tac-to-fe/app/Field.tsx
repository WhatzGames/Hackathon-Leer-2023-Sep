import {useGameStore} from '@/app/game-store';
import {cn} from '@/lib/cn';
import {Player} from '@/app/Player';

export type FieldProps = {
  board: number;
  field: number;
  view: boolean;
  size?: number;
}

export default function Field({board, field, view, size = 64, children}: React.PropsWithChildren<FieldProps>) {
  const setField = useGameStore(state => state.setField);

  const handleClick = () => setField(board, field);

  return (
    <button className={cn(
      'border border-slate-400 rounded-lg text-3xl flex justify-center items-center',
      !view && 'hover:bg-slate-200 hover:animate-pulse'
    )}
            onClick={view ? void 0 : handleClick}
            style={{width: size, height: size}}>
      {children
        ? <Player>{children}</Player>
        : null
      }
    </button>
  );
};
