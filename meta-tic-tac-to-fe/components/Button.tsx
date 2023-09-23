import {cn} from '@/lib/cn';

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement>

export function Button({className, children, ...props}: React.PropsWithChildren<ButtonProps>) {
  return (
    <button className={cn('border border-slate-400 rounded-xl p-4 hover:bg-slate-200 disabled:opacity-70', className)}
            {...props}>
      {children}
    </button>
  );
}