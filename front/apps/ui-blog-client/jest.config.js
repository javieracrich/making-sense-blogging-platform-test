module.exports = {
  name: 'ui-blog-client',
  preset: '../../jest.config.js',
  coverageDirectory: '../../coverage/apps/ui-blog-client/',
  snapshotSerializers: [
    'jest-preset-angular/AngularSnapshotSerializer.js',
    'jest-preset-angular/HTMLCommentSerializer.js'
  ]
};
