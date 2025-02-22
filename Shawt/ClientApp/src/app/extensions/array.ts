declare global {
  interface Array<T> {
    groupBy<K>(prop): any;
    groupByCount<K>(prop): any;
  }
}

Array.prototype.groupBy = function (prop) {
  return this.reduce(function (groups, item) {
    const val = item[prop]
    groups[val] = groups[val] || []
    groups[val].push(item)
    return groups;
  }, {})
}

Array.prototype.groupByCount = function (prop) {
  return this.reduce(function (groups, item) {
    const val = item[prop];
    groups[val] = groups[val] != null ? groups[val] + 1 : 1;
    //groups[val] = groups[val] + 1;
    return groups;
  }, {})
}

export { };
