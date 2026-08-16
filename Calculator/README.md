# Calculator

There are many times when the streamer or someone in chat needs to do some math on the spot.
This Streamer.bot action lets people in chat evaluate math expressions without leaving the stream. 
Use `=` followed by an expression and the bot will reply with the result.
It is also capable of doing unit conversions.

Examples:
- Add and multiply with parentheses - `= (3+4)*2`
- Use a function and a constant - `= sqrt(16)+pi`
- Implicit multiplication - `= 2pi` or `= 2(3+4)`
- Convert degrees before using trig functions - `= sin(rad(90))`
- Convert units - `= 5 ft to m`
- Convert with an assumed target unit - `=65f`

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

## Unit Conversion
The same `=` command can also convert units. This is separate from the math evaluator: the value has to be a number,
not an expression.

To convert explicitly, use:

`= <number> <from unit> to <to unit>`

Examples:
- `= 5 ft to m`
- `= 100 km to mi`
- `= 32 f to c`
- `= 2 cups to ml`

If you only give a number and a unit, it converts to an assumed target in the other common system:

- `=65f` becomes `65 F = 18.333 C`
- `= 5 ft` becomes `5 ft = 1.524 m`
- `= 2 kg` becomes `2 kg = 4.409 lb`

Results show both sides and are rounded to 3 decimal places. Units in the same family can be converted to each other.
Trying to convert across families, such as feet to celsius, gets an error.

### Length
mm, cm, m, km, in, ft, yd, mi

Assumed targets: mm and cm to in, m to ft, km to mi, in to cm, ft and yd to m, mi to km.

### Mass
mg, g, kg, oz, lb

Assumed targets: mg and g to oz, kg to lb, oz to g, lb to kg.

### Temperature
C, F, K

Assumed targets: F to C, C to F, K to C.

### Volume
ml, L, tsp, tbsp, cup, fl oz, gal

These are US customary units. Assumed targets: ml to fl oz, L to gal, tsp, tbsp, cup, and fl oz to ml, gal to L.

### Time
sec, min, hr, day

Assumed targets: sec to min, min to hr, hr to min, day to hr.

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


