# Calculator

There are many times when the streamer or someone in chat needs to do some math on the spot.
This Streamer.bot action lets people in chat evaluate math expressions without leaving the stream. 
Use `=` followed by an expression and the bot will reply with the result.

Examples:
- Add and multiply with parentheses - `= (3+4)*2`
- Use a function and a constant - `= sqrt(16)+pi`
- Implicit multiplication - `= 2pi` or `= 2(3+4)`
- Convert degrees before using trig functions - `= sin(rad(90))`

If someone uses `=` with nothing after it, the bot replies with a short usage message.

## Operators
- `+` `-` `*` `/` for the usual arithmetic
- `%` for remainder (modulo)
- `^` for exponents, such as `= 2^8`
- Parentheses to group parts of an expression
- Unary plus and minus, such as `= -3 + 5`

## Constants
- `pi`
- `e`
- `tau` (2 * pi)

## Functions
- General: `abs`, `sqrt`, `floor`, `ceil` (or `ceiling`), `round`, `sign`, `min`, `max`, `pow`
- Exponential and log: `exp`, `ln`, `log`, `log10`
- Trigonometry: `sin`, `cos`, `tan`, `asin`, `acos`, `atan`, `atan2`, `sinh`, `cosh`, `tanh`
- Conversion: `deg` (radians to degrees), `rad` (degrees to radians)

`log(x)` is the natural log. `log(x, b)` is log base `b`. Trigonometric functions use radians, so convert degrees with
`rad()` first if that is what you have.

If the expression cannot be evaluated, the bot replies with a short error instead of a result. That includes things like
division by zero, an unknown name, or a missing parenthesis.

## Install
Click "Calculator.sb" in the repo then click the "Download" button to download it to your computer.

![](assets/github-download.png)

In Streamer.bot on your computer click the "Import" menu to open the import dialog.

![](assets/streamerbot-import.png)

On your computer, drag the "Calculator.sb" file and drop it into the window. Click the "Import" button.

![](assets/import-dialog.png)

(There is a general video tutorial on importing into Streamer.bot here: https://youtu.be/gHqw3gwpbco)

## Note
This action includes a command. Imported commands are disabled by default, so be sure to enable it.


