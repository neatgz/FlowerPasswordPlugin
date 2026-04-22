const md5 = require('./FlowerPassword/utils/md5.min.js');
const STR1 = 'snow';
const STR2 = 'kise';
const STR3 = 'sunlovesnow1990090127xykab';

function generate(keyword, code) {
  var md5one = md5(keyword, code);
  var md5two = md5(md5one, STR1);
  var md5three = md5(md5one, STR2);
  var rule = md5three.split('');
  var source = md5two.split('');
  for (var i = 0; i < 32; i++) {
    if (isNaN(source[i])) {
      if (STR3.search(rule[i]) > -1) {
        source[i] = source[i].toUpperCase();
      }
    }
  }
  var pwd32 = source.join('');
  var firstChar = pwd32.slice(0, 1);
  if (isNaN(firstChar)) {
    var pwd = pwd32.slice(0, 16);
  } else {
    var pwd = 'K' + pwd32.slice(1, 16);
  }
  return { md5one, md5two, md5three, pwd };
}

console.log(generate('test', 'example'));
