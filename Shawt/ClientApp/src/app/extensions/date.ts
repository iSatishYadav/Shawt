declare global {
  interface Date {
    toFormatString(format: string): string;
  }
}

Date.prototype.toFormatString = function (format: string) {
  let dd = this.getDate();

  let mm = this.getMonth() + 1;
  const yyyy = this.getFullYear();
  if (dd < 10) {
    dd = `0${dd}`;
  }

  if (mm < 10) {
    mm = `0${mm}`;
  }
  return format.replace("dd", dd).replace("MM", mm).replace("yyyy", yyyy);
}

export { };
