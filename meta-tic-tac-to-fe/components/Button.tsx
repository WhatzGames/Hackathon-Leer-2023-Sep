import {cn} from '@/lib/cn';

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement>

export function Button({className, children, ...props}: React.PropsWithChildren<ButtonProps>) {
  return (
    <button className={cn('bg-amber-500 border border-amber-600 rounded-xl p-4 hover:bg-amber-400 disabled:opacity-70', className)}
            {...props}>
      {children}
    </button>
  );
}